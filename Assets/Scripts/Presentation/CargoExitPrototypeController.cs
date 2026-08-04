using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BannoyasGames.CargoExit.Core;
using TMPro;
using UnityEngine;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class CargoExitPrototypeController : MonoBehaviour
    {
        private const string StrongEmployeeId = "employee-strength";
        private const string CarefulEmployeeId = "employee-care";
        private const string HeavyParcelId = "cargo-heavy";
        private const string FragileParcelId = "cargo-fragile";

        [SerializeField] private TMP_FontAsset regularFont;
        [SerializeField] private TMP_FontAsset boldFont;

        private readonly List<CargoParcelView> boxes = new();
        private readonly Dictionary<string, CargoEmployeeView> employees = new();
        private readonly Dictionary<string, CargoAttributes> processingAttributes = new();
        private readonly Dictionary<string, WorkStationType> processingStations = new();
        private readonly Dictionary<string, CargoProcessingResult> processingResults = new();
        private readonly HashSet<string> processedParcels = new();
        private CargoExitPrototypeView view;
        private CargoExitWorkTestView workTestView;
        private CargoSortSession session;
        private WorkAssignmentSession assignmentSession;
        private int combo;
        private bool roundEnding;
        private bool processingWorkTest;

        public Transform CanvasTransform => view.Canvas.transform;

        private void Awake()
        {
            UnityEngine.Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            UiElementFactory.ConfigureFonts(regularFont, boldFont);
            CargoExitPrototypeViewFactory.EnsureCamera(transform);

            if (CargoExitPrototypeViewFactory.TryBind(transform, out view))
            {
                RemoveSerializedPreviewBoxes();
            }
            else
            {
                view = CargoExitPrototypeViewFactory.Create(transform);
            }

            workTestView = CargoExitPrototypeViewFactory.CreateWorkTest(
                view.Canvas.transform,
                TryConfirmAssignment,
                HideAssignmentWarning,
                ConfirmImperfectAssignment);
            StartWorkTest();
        }

        public void BuildScenePreview()
        {
            UiElementFactory.ConfigureFonts(regularFont, boldFont);
            CargoExitPrototypeViewFactory.EnsureCamera(transform);
            view = CargoExitPrototypeViewFactory.Create(transform);
            StartRound();
        }

        public void ConfigureFonts(
            TMP_FontAsset regular,
            TMP_FontAsset bold)
        {
            regularFont = regular;
            boldFont = bold;
            UiElementFactory.ConfigureFonts(regularFont, boldFont);
        }

        public void BeginEmployeeAssignment(CargoEmployeeView employee)
        {
            employee.RectTransform.localScale = Vector3.one * 1.07f;
            workTestView.StatusLabel.text = "ELEGÍ UNA ESTACIÓN";
            ResetWorkStationColors();
        }

        public void ContinueEmployeeAssignment(
            CargoEmployeeView employee,
            Vector3 worldTarget)
        {
            var local = (Vector2)view.CanvasRect.InverseTransformPoint(worldTarget);
            var closestStation = workTestView.Stations.Values
                .OrderBy(station => Vector2.Distance(
                    local,
                    (Vector2)view.CanvasRect.InverseTransformPoint(
                        station.EmployeeSlot.position)))
                .First();
            var stationCenter = (Vector2)view.CanvasRect.InverseTransformPoint(
                closestStation.EmployeeSlot.position);
            var distance = Vector2.Distance(local, stationCenter);

            if (distance < 135f)
            {
                var attraction = 1f - Mathf.Clamp01(distance / 135f);
                attraction = attraction * attraction * 0.38f;
                local = Vector2.Lerp(local, stationCenter, attraction);
                HighlightWorkStation(closestStation.Type);
            }
            else
            {
                ResetWorkStationColors();
            }

            employee.RectTransform.anchoredPosition = local;
            employee.RectTransform.localEulerAngles = new Vector3(
                0f,
                0f,
                Mathf.LerpAngle(
                    employee.RectTransform.localEulerAngles.z,
                    0f,
                    0.18f));
        }

        public void EndEmployeeAssignment(
            CargoEmployeeView employee,
            Vector2 pointerPosition)
        {
            var station = workTestView.Stations.Values.FirstOrDefault(
                candidate => RectTransformUtility.RectangleContainsScreenPoint(
                    candidate.Surface,
                    pointerPosition));

            if (station != null &&
                assignmentSession.TryAssign(employee.EmployeeId, station.Type))
            {
                workTestView.StatusLabel.text = assignmentSession.IsComplete
                    ? "ASIGNACIÓN LISTA PARA CONFIRMAR"
                    : "FALTA OCUPAR UNA ESTACIÓN";
                workTestView.ConfirmButton.interactable = assignmentSession.IsComplete;
                MoveEmployeesToCurrentAssignments(0.23f);
                ResetWorkStationColors();
                return;
            }

            workTestView.StatusLabel.text = "SOLTÁ EL EMPLEADO EN UNA ESTACIÓN";
            MoveEmployeeToCurrentAssignment(employee, 0.3f);
            ResetWorkStationColors();
        }

        public void BeginSort(CargoParcelView box)
        {
            if (processingWorkTest)
            {
                BeginCargoProcessing(box);
                return;
            }

            box.RectTransform.localScale = Vector3.one * 1.07f;
            view.StatusLabel.text = $"BUSCÁ EL PALLET {box.Destination}";
            HighlightDestination(box.Destination);
        }

        public void ContinueSort(CargoParcelView box, Vector3 worldTarget)
        {
            if (processingWorkTest)
            {
                ContinueCargoProcessing(box, worldTarget);
                return;
            }

            var local = (Vector2)view.CanvasRect.InverseTransformPoint(worldTarget);
            var targetPallet = view.Pallets[box.Destination];
            var palletCenter = (Vector2)view.CanvasRect.InverseTransformPoint(
                targetPallet.Surface.position);
            var distance = Vector2.Distance(local, palletCenter);

            if (distance < 135f)
            {
                var attraction = 1f - Mathf.Clamp01(distance / 135f);
                attraction = attraction * attraction * 0.38f;
                local = Vector2.Lerp(local, palletCenter, attraction);
            }

            box.RectTransform.anchoredPosition = local;
            box.RectTransform.localEulerAngles = new Vector3(
                0f,
                0f,
                Mathf.LerpAngle(
                    box.RectTransform.localEulerAngles.z,
                    0f,
                    0.18f));
        }

        public void EndSort(CargoParcelView box, Vector2 pointerPosition)
        {
            if (processingWorkTest)
            {
                EndCargoProcessing(box, pointerPosition);
                return;
            }

            var droppedPallet = view.Pallets.Values.FirstOrDefault(
                pallet => RectTransformUtility.RectangleContainsScreenPoint(
                    pallet.Surface,
                    pointerPosition));

            if (droppedPallet != null)
            {
                var result = session.TryPlace(
                    box.ParcelId,
                    droppedPallet.Destination);
                if (result.Accepted)
                {
                    combo++;
                    UpdateCombo();
                    view.StatusLabel.text = combo == 1
                        ? "ENCAJÓ"
                        : $"BIEN ×{combo}";
                    droppedPallet.Counter.text =
                        $"{result.SortedAtDestination} / {result.RequiredAtDestination}";
                    ResetPalletColors();
                    StartCoroutine(SnapIntoPallet(
                        box,
                        droppedPallet,
                        result.SortedAtDestination - 1,
                        result.RoundComplete));
                    return;
                }

                view.StatusLabel.text = PlacementFailureMessage(
                    result.Status,
                    box.Destination,
                    droppedPallet.Destination);
            }
            else
            {
                view.StatusLabel.text =
                    $"La caja {box.Destination} va en su pallet";
            }

            combo = 0;
            UpdateCombo();
            HighlightDestination(box.Destination);
            StartCoroutine(ReturnToPile(box));
        }

        private void StartWorkTest()
        {
            processingWorkTest = false;
            processedParcels.Clear();
            processingAttributes.Clear();
            processingStations.Clear();
            processingResults.Clear();
            employees.Clear();

            assignmentSession = new WorkAssignmentSession(new[]
            {
                new WorkEmployee(StrongEmployeeId, EmployeeSkill.Strength),
                new WorkEmployee(CarefulEmployeeId, EmployeeSkill.Care)
            });

            employees.Add(
                StrongEmployeeId,
                CargoExitPrototypeViewFactory.CreateEmployee(
                    workTestView.EmployeePool,
                    this,
                    StrongEmployeeId,
                    "EMPLEADO FUERTE",
                    "Habilidad: FUERZA",
                    UiElementFactory.Hex("#F7C843"),
                    new Vector2(-112f, -18f)));
            employees.Add(
                CarefulEmployeeId,
                CargoExitPrototypeViewFactory.CreateEmployee(
                    workTestView.EmployeePool,
                    this,
                    CarefulEmployeeId,
                    "EMPLEADO CUIDADOSO",
                    "Habilidad: CUIDADO",
                    UiElementFactory.Hex("#65D6AD"),
                    new Vector2(112f, -18f)));

            workTestView.ConfirmButton.interactable = false;
            ResetWorkStationColors();
        }

        private void TryConfirmAssignment()
        {
            if (!assignmentSession.IsComplete)
            {
                return;
            }

            if (!assignmentSession.HasImperfectAssignment)
            {
                ConfirmAssignmentAndStartProcessing();
                return;
            }

            var warnings = new List<string>();
            if (!assignmentSession.IsCompatible(WorkStationType.HeavyCargo))
            {
                warnings.Add("La estación pesada trabajará con menor eficiencia.");
            }

            if (!assignmentSession.IsCompatible(WorkStationType.FragileCargo))
            {
                warnings.Add("La carga frágil puede dañarse.");
            }

            workTestView.WarningLabel.text = string.Join("\n\n", warnings);
            workTestView.WarningPanel.gameObject.SetActive(true);
            workTestView.WarningPanel.SetAsLastSibling();
        }

        private void HideAssignmentWarning()
        {
            workTestView.WarningPanel.gameObject.SetActive(false);
            workTestView.StatusLabel.text = "PODÉS CAMBIAR LA ASIGNACIÓN";
        }

        private void ConfirmImperfectAssignment()
        {
            workTestView.WarningPanel.gameObject.SetActive(false);
            ConfirmAssignmentAndStartProcessing();
        }

        private void ConfirmAssignmentAndStartProcessing()
        {
            if (!assignmentSession.Confirm())
            {
                return;
            }

            foreach (var employee in employees.Values)
            {
                employee.SetInteraction(false);
            }

            processingWorkTest = true;
            workTestView.TitleLabel.text = "PROCESÁ LA CARGA";
            workTestView.StatusLabel.text = "ELEGÍ UNA CAJA";
            workTestView.HintLabel.text =
                "Arrastrá cada caja a la estación que indica su atributo";
            workTestView.EmployeePool.gameObject.SetActive(false);
            workTestView.ConfirmButton.gameObject.SetActive(false);
            workTestView.ProcessingPile.gameObject.SetActive(true);

            processingAttributes.Add(
                HeavyParcelId,
                new CargoAttributes(CargoWeight.Heavy, false));
            processingStations.Add(HeavyParcelId, WorkStationType.HeavyCargo);
            processingAttributes.Add(
                FragileParcelId,
                new CargoAttributes(CargoWeight.Standard, true));
            processingStations.Add(FragileParcelId, WorkStationType.FragileCargo);

            CreateProcessingBox(
                HeavyParcelId,
                "PESADA",
                UiElementFactory.Hex("#F7C843"),
                new Vector2(-112f, -22f),
                -4f);
            CreateProcessingBox(
                FragileParcelId,
                "FRÁGIL",
                UiElementFactory.Hex("#FF8A65"),
                new Vector2(112f, -22f),
                4f);
            ResetWorkStationColors();
        }

        private void BeginCargoProcessing(CargoParcelView box)
        {
            var targetStation = processingStations[box.ParcelId];
            box.RectTransform.localScale = Vector3.one * 1.07f;
            workTestView.StatusLabel.text = targetStation == WorkStationType.HeavyCargo
                ? "LLEVÁ LA CAJA PESADA A SU ESTACIÓN"
                : "LLEVÁ LA CAJA FRÁGIL A SU ESTACIÓN";
            HighlightWorkStation(targetStation);
        }

        private void ContinueCargoProcessing(
            CargoParcelView box,
            Vector3 worldTarget)
        {
            var local = (Vector2)view.CanvasRect.InverseTransformPoint(worldTarget);
            var expectedStation = processingStations[box.ParcelId];
            var targetStation = workTestView.Stations.Values
                .OrderBy(station => Vector2.Distance(
                    local,
                    (Vector2)view.CanvasRect.InverseTransformPoint(
                        station.CargoSlot.position)))
                .First();
            var stationCenter = (Vector2)view.CanvasRect.InverseTransformPoint(
                targetStation.CargoSlot.position);
            var distance = Vector2.Distance(local, stationCenter);

            if (distance < 135f)
            {
                var attraction = 1f - Mathf.Clamp01(distance / 135f);
                attraction = attraction * attraction * 0.38f;
                local = Vector2.Lerp(local, stationCenter, attraction);
                HighlightWorkStation(targetStation.Type);
            }
            else
            {
                HighlightWorkStation(expectedStation);
            }

            box.RectTransform.anchoredPosition = local;
            box.RectTransform.localEulerAngles = new Vector3(
                0f,
                0f,
                Mathf.LerpAngle(
                    box.RectTransform.localEulerAngles.z,
                    0f,
                    0.18f));
        }

        private void EndCargoProcessing(
            CargoParcelView box,
            Vector2 pointerPosition)
        {
            var droppedStation = workTestView.Stations.Values.FirstOrDefault(
                station => RectTransformUtility.RectangleContainsScreenPoint(
                    station.Surface,
                    pointerPosition));
            var expectedStation = processingStations[box.ParcelId];

            if (droppedStation == null)
            {
                workTestView.StatusLabel.text = expectedStation ==
                    WorkStationType.HeavyCargo
                    ? "LA CAJA PESADA VA EN LA ESTACIÓN PESADA"
                    : "LA CAJA FRÁGIL VA EN LA ESTACIÓN FRÁGIL";
                HighlightWorkStation(expectedStation);
                StartCoroutine(ReturnToPile(box));
                return;
            }

            var result = CargoProcessingRule.Process(
                processingAttributes[box.ParcelId],
                droppedStation.Type,
                assignmentSession.GetAssignedSkill(droppedStation.Type));
            processingResults[box.ParcelId] = result;
            processedParcels.Add(box.ParcelId);

            var resultLabel = ProcessingResultLabel(result);
            var resultColor = ProcessingResultColor(result);
            box.SetVisualResult(
                resultLabel,
                resultColor,
                UiElementFactory.Hex("#172238"));
            droppedStation.ResultLabel.text = resultLabel;
            droppedStation.ResultLabel.color = UiElementFactory.Hex("#172238");
            workTestView.StatusLabel.text = ProcessingStatus(result);
            ResetWorkStationColors();

            StartCoroutine(SnapIntoWorkStation(
                box,
                droppedStation,
                processedParcels.Count == processingAttributes.Count));
        }

        private void CreateProcessingBox(
            string parcelId,
            string label,
            Color color,
            Vector2 position,
            float rotation)
        {
            var gameObject = new GameObject(
                parcelId,
                typeof(RectTransform),
                typeof(CargoParcelView));
            gameObject.transform.SetParent(workTestView.ProcessingPile, false);

            var parcelView = gameObject.GetComponent<CargoParcelView>();
            parcelView.InitializeForProcessing(
                this,
                parcelId,
                color,
                position,
                rotation,
                label);
            boxes.Add(parcelView);
        }

        private void MoveEmployeesToCurrentAssignments(float duration)
        {
            foreach (var employee in employees.Values)
            {
                MoveEmployeeToCurrentAssignment(employee, duration);
            }
        }

        private void MoveEmployeeToCurrentAssignment(
            CargoEmployeeView employee,
            float duration)
        {
            if (assignmentSession.TryGetStationFor(
                    employee.EmployeeId,
                    out var stationType))
            {
                StartCoroutine(MoveEmployee(
                    employee,
                    workTestView.Stations[stationType].EmployeeSlot,
                    Vector2.zero,
                    Vector3.one * 0.86f,
                    duration));
                return;
            }

            StartCoroutine(MoveEmployee(
                employee,
                employee.HomeParent,
                employee.HomePosition,
                Vector3.one,
                duration));
        }

        private IEnumerator MoveEmployee(
            CargoEmployeeView employee,
            Transform targetParent,
            Vector2 targetAnchoredPosition,
            Vector3 targetScale,
            float duration)
        {
            employee.SetInteraction(false);
            var startPosition = employee.RectTransform.position;
            var startScale = employee.RectTransform.localScale;
            var targetPosition = targetParent.TransformPoint(targetAnchoredPosition);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                employee.RectTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    eased);
                employee.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    targetScale,
                    eased);
                employee.RectTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    Mathf.LerpAngle(
                        employee.RectTransform.localEulerAngles.z,
                        0f,
                        eased));
                yield return null;
            }

            employee.PlaceAt(targetParent, targetAnchoredPosition, targetScale);
            employee.SetInteraction(!assignmentSession.IsConfirmed);
        }

        private IEnumerator SnapIntoWorkStation(
            CargoParcelView box,
            CargoWorkStationView station,
            bool completesTest)
        {
            box.SetInteraction(false);
            var startPosition = box.RectTransform.position;
            var startScale = box.RectTransform.localScale;
            var targetPosition = station.CargoSlot.position;

            const float duration = 0.23f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                var punch = Mathf.Sin(t * Mathf.PI) * 0.1f;
                box.RectTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    eased);
                box.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    Vector3.one * (0.86f + punch),
                    eased);
                box.RectTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    Mathf.LerpAngle(
                        box.RectTransform.localEulerAngles.z,
                        0f,
                        eased));
                yield return null;
            }

            box.RectTransform.SetParent(station.CargoSlot, false);
            box.RectTransform.anchoredPosition = Vector2.zero;
            box.RectTransform.localScale = Vector3.one * 0.86f;
            box.RectTransform.localEulerAngles = Vector3.zero;

            if (completesTest)
            {
                CompleteWorkTest();
            }
        }

        private void CompleteWorkTest()
        {
            workTestView.TitleLabel.text = "PRUEBA COMPLETADA";
            workTestView.StatusLabel.text = "LAS DOS CAJAS FUERON PROCESADAS";
            workTestView.HintLabel.text = string.Empty;
            workTestView.ProcessingPile.gameObject.SetActive(false);
            workTestView.FinalLabel.text =
                $"RESULTADOS\n\n" +
                $"Carga pesada: {ProcessingResultLabel(processingResults[HeavyParcelId])}\n" +
                $"Carga frágil: {ProcessingResultLabel(processingResults[FragileParcelId])}";
            workTestView.FinalPanel.gameObject.SetActive(true);
            workTestView.FinalPanel.SetAsLastSibling();
            ResetWorkStationColors();
        }

        private void HighlightWorkStation(WorkStationType stationType)
        {
            foreach (var station in workTestView.Stations.Values)
            {
                var color = station.Color;
                color.a = station.Type == stationType ? 0.62f : 0.18f;
                station.Background.color = color;
            }
        }

        private void ResetWorkStationColors()
        {
            foreach (var station in workTestView.Stations.Values)
            {
                var color = station.Color;
                color.a = 0.3f;
                station.Background.color = color;
            }
        }

        private static string ProcessingResultLabel(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient => "INEFICIENTE",
                CargoProcessingResult.Damaged => "DAÑADA",
                _ => "NORMAL"
            };
        }

        private static Color ProcessingResultColor(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient => UiElementFactory.Hex("#F7C843"),
                CargoProcessingResult.Damaged => UiElementFactory.Hex("#EF4444"),
                _ => UiElementFactory.Hex("#65D6AD")
            };
        }

        private static string ProcessingStatus(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient =>
                    "PROCESAMIENTO INEFICIENTE",
                CargoProcessingResult.Damaged =>
                    "LA CAJA QUEDÓ DAÑADA",
                _ => "PROCESAMIENTO NORMAL"
            };
        }

        private void RemoveSerializedPreviewBoxes()
        {
            foreach (var preview in GetComponentsInChildren<CargoParcelView>(true))
            {
                preview.gameObject.SetActive(false);
                Destroy(preview.gameObject);
            }

            boxes.Clear();
        }

        private void StartRound()
        {
            combo = 0;
            roundEnding = false;
            view.StatusLabel.text = "ELEGÍ UNA CAJA";
            UpdateCombo();
            ResetPalletColors();

            var destinations = new[]
            {
                CargoDestination.D,
                CargoDestination.A,
                CargoDestination.F,
                CargoDestination.C,
                CargoDestination.G,
                CargoDestination.B,
                CargoDestination.E,
                CargoDestination.C,
                CargoDestination.E,
                CargoDestination.A,
                CargoDestination.G,
                CargoDestination.D,
                CargoDestination.B,
                CargoDestination.F
            };
            var plan = destinations
                .Select((destination, index) => new CargoParcelPlan(
                    $"Box {index + 1}",
                    destination));
            session = new CargoSortSession(plan);

            foreach (var pair in view.Pallets)
            {
                pair.Value.Counter.text =
                    $"0 / {session.GetRequiredCount(pair.Key)}";
            }

            var positions = new[]
            {
                new Vector2(-44f, 155f),
                new Vector2(45f, 130f),
                new Vector2(-52f, 105f),
                new Vector2(48f, 80f),
                new Vector2(-38f, 55f),
                new Vector2(42f, 30f),
                new Vector2(-50f, 5f),
                new Vector2(51f, -20f),
                new Vector2(-41f, -45f),
                new Vector2(44f, -70f),
                new Vector2(-51f, -95f),
                new Vector2(48f, -120f),
                new Vector2(-36f, -145f),
                new Vector2(38f, -170f)
            };
            var rotations = new[]
            {
                -5f, 4f, -3f, 6f, -4f, 3f, -6f,
                5f, -3f, 4f, -5f, 3f, -4f, 5f
            };

            for (var i = 0; i < destinations.Length; i++)
            {
                CreateBox(
                    $"Box {i + 1}",
                    destinations[i],
                    positions[i],
                    rotations[i]);
            }
        }

        private void CreateBox(
            string name,
            CargoDestination destination,
            Vector2 position,
            float rotation)
        {
            var gameObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CargoParcelView));
            gameObject.transform.SetParent(view.PileSurface, false);

            var parcelView = gameObject.GetComponent<CargoParcelView>();
            parcelView.Initialize(
                this,
                name,
                destination,
                CargoExitPrototypeViewFactory.DestinationColor(destination),
                position,
                rotation);
            boxes.Add(parcelView);
        }

        private IEnumerator ReturnToPile(CargoParcelView box)
        {
            box.SetInteraction(false);
            var startPosition = box.RectTransform.position;
            var startScale = box.RectTransform.localScale;
            var targetPosition = box.HomeWorldPosition;

            const float duration = 0.3f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                box.RectTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    eased);
                box.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    Vector3.one,
                    eased);
                yield return null;
            }

            box.RestoreHome();
            if (processingWorkTest)
            {
                ResetWorkStationColors();
            }
            else
            {
                ResetPalletColors();
            }
        }

        private IEnumerator SnapIntoPallet(
            CargoParcelView box,
            CargoPalletView pallet,
            int slotIndex,
            bool completesRound)
        {
            box.SetInteraction(false);
            var startPosition = box.RectTransform.position;
            var startScale = box.RectTransform.localScale;
            var slot = new Vector2(slotIndex == 0 ? -29f : 29f, -3f);
            var targetPosition = pallet.Surface.TransformPoint(slot);

            const float duration = 0.23f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                var punch = Mathf.Sin(t * Mathf.PI) * 0.1f;
                box.RectTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    eased);
                box.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    Vector3.one * (0.56f + punch),
                    eased);
                box.RectTransform.localEulerAngles = new Vector3(
                    0f,
                    0f,
                    Mathf.LerpAngle(
                        box.RectTransform.localEulerAngles.z,
                        0f,
                        eased));
                yield return null;
            }

            box.RectTransform.SetParent(pallet.Surface, false);
            box.RectTransform.anchoredPosition = slot;
            box.RectTransform.localScale = Vector3.one * 0.56f;
            box.RectTransform.localEulerAngles = Vector3.zero;

            if (!roundEnding && completesRound)
            {
                roundEnding = true;
                StartCoroutine(CompleteRound());
            }
        }

        private IEnumerator CompleteRound()
        {
            view.StatusLabel.text = "¡DEPÓSITO ORDENADO!";
            foreach (var pallet in view.Pallets.Values)
            {
                StartCoroutine(PulsePallet(pallet));
            }

            yield return new WaitForSecondsRealtime(1.4f);

            foreach (var box in boxes)
            {
                Destroy(box.gameObject);
            }

            boxes.Clear();
            yield return null;
            StartRound();
        }

        private IEnumerator PulsePallet(CargoPalletView pallet)
        {
            var startScale = pallet.Surface.localScale;
            for (var elapsed = 0f; elapsed < 0.28f; elapsed += Time.unscaledDeltaTime)
            {
                var normalized = elapsed / 0.28f;
                var pulse = 1f + Mathf.Sin(normalized * Mathf.PI) * 0.06f;
                pallet.Surface.localScale = startScale * pulse;
                yield return null;
            }

            pallet.Surface.localScale = startScale;
        }

        private void HighlightDestination(CargoDestination destination)
        {
            foreach (var pallet in view.Pallets.Values)
            {
                var color = pallet.Color;
                color.a = pallet.Destination == destination ? 0.56f : 0.16f;
                pallet.Background.color = color;
            }
        }

        private void ResetPalletColors()
        {
            foreach (var pallet in view.Pallets.Values)
            {
                var color = pallet.Color;
                color.a = 0.24f;
                pallet.Background.color = color;
            }
        }

        private void UpdateCombo()
        {
            view.ComboLabel.text = combo > 1 ? $"RACHA ×{combo}" : string.Empty;
        }

        private static string PlacementFailureMessage(
            SortPlacementStatus status,
            CargoDestination expected,
            CargoDestination dropped)
        {
            return status switch
            {
                SortPlacementStatus.PalletFull =>
                    $"El pallet {dropped} ya está completo",
                SortPlacementStatus.AlreadySorted =>
                    "Esa caja ya fue registrada",
                SortPlacementStatus.UnknownParcel =>
                    "No pudimos identificar esa caja",
                _ => $"Ese es {dropped}; buscá {expected}"
            };
        }
    }
}
