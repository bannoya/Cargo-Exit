using System.Collections;
using System.Collections.Generic;
using BannoyasGames.CargoExit.Application;
using BannoyasGames.CargoExit.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class CargoProcessingController : MonoBehaviour
    {
        private const string HeavyParcelId = "cargo-heavy";
        private const string FragileParcelId = "cargo-fragile";

        [SerializeField] private TMP_FontAsset regularFont;
        [SerializeField] private TMP_FontAsset boldFont;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text statusLabel;
        [SerializeField] private RectTransform cargoPool;
        [SerializeField] private RectTransform heavyStationSurface;
        [SerializeField] private RectTransform fragileStationSurface;
        [SerializeField] private RectTransform heavyCargoSlot;
        [SerializeField] private RectTransform fragileCargoSlot;
        [SerializeField] private Image heavyStationBackground;
        [SerializeField] private Image fragileStationBackground;
        [SerializeField] private Text heavyResultLabel;
        [SerializeField] private Text fragileResultLabel;
        [SerializeField] private GameObject completionPanel;
        [SerializeField] private Text completionLabel;
        [SerializeField] private Button continueButton;

        private readonly Dictionary<string, CargoAttributes> attributes = new();
        private readonly Dictionary<string, WorkStationType> expectedStations = new();
        private readonly Dictionary<string, CargoProcessingResult> results = new();
        private readonly List<CargoParcelView> boxes = new();
        private Color heavyStationColor;
        private Color fragileStationColor;

        public Transform CanvasTransform => canvas.transform;

        private void Awake()
        {
            UiElementFactory.ConfigureFonts(regularFont, boldFont);
            heavyStationColor = heavyStationBackground.color;
            fragileStationColor = fragileStationBackground.color;
            completionPanel.SetActive(false);
            continueButton.interactable = false;
            continueButton.onClick.AddListener(Continue);

            attributes.Add(
                HeavyParcelId,
                new CargoAttributes(CargoWeight.Heavy, false));
            expectedStations.Add(HeavyParcelId, WorkStationType.HeavyCargo);
            attributes.Add(
                FragileParcelId,
                new CargoAttributes(CargoWeight.Standard, true));
            expectedStations.Add(FragileParcelId, WorkStationType.FragileCargo);

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
            ResetStationColors();
        }

        private void Start()
        {
            if (CargoSessionFlow.Instance.State.AssignmentConfirmed)
            {
                statusLabel.text = "ELEGÍ UNA CAJA";
                return;
            }

            statusLabel.text = "FALTA CONFIRMAR LA ASIGNACIÓN DE EMPLEADOS";
            foreach (var box in boxes)
            {
                box.SetInteraction(false);
            }
        }

        public void BeginSort(CargoParcelView box)
        {
            var expectedStation = expectedStations[box.ParcelId];
            box.RectTransform.localScale = Vector3.one * 1.07f;
            statusLabel.text = expectedStation == WorkStationType.HeavyCargo
                ? "LLEVÁ LA CAJA PESADA A UNA ESTACIÓN"
                : "LLEVÁ LA CAJA FRÁGIL A UNA ESTACIÓN";
            HighlightStation(expectedStation);
        }

        public void ContinueSort(CargoParcelView box, Vector3 worldTarget)
        {
            var canvasRect = (RectTransform)canvas.transform;
            var local = (Vector2)canvasRect.InverseTransformPoint(worldTarget);
            var heavyCenter = (Vector2)canvasRect.InverseTransformPoint(
                heavyCargoSlot.position);
            var fragileCenter = (Vector2)canvasRect.InverseTransformPoint(
                fragileCargoSlot.position);
            var heavyDistance = Vector2.Distance(local, heavyCenter);
            var fragileDistance = Vector2.Distance(local, fragileCenter);
            var targetStation = heavyDistance <= fragileDistance
                ? WorkStationType.HeavyCargo
                : WorkStationType.FragileCargo;
            var stationCenter = targetStation == WorkStationType.HeavyCargo
                ? heavyCenter
                : fragileCenter;
            var distance = Mathf.Min(heavyDistance, fragileDistance);

            if (distance < 135f)
            {
                var attraction = 1f - Mathf.Clamp01(distance / 135f);
                attraction = attraction * attraction * 0.38f;
                local = Vector2.Lerp(local, stationCenter, attraction);
                HighlightStation(targetStation);
            }
            else
            {
                HighlightStation(expectedStations[box.ParcelId]);
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
            var station = StationAt(pointerPosition);
            if (!station.HasValue)
            {
                statusLabel.text = "SOLTÁ LA CAJA EN UNA ESTACIÓN";
                HighlightStation(expectedStations[box.ParcelId]);
                StartCoroutine(ReturnToPile(box));
                return;
            }

            var state = CargoSessionFlow.Instance.State;
            var result = CargoProcessingRule.Process(
                attributes[box.ParcelId],
                station.Value,
                state.GetAssignedSkill(station.Value));
            results[box.ParcelId] = result;

            box.SetVisualResult(
                ResultLabel(result),
                ResultColor(result),
                UiElementFactory.Hex("#172238"));
            ResultText(station.Value).text = ResultLabel(result);
            statusLabel.text = ResultStatus(result);
            ResetStationColors();
            StartCoroutine(SnapIntoStation(
                box,
                station.Value,
                results.Count == attributes.Count));
        }

        private WorkStationType? StationAt(Vector2 pointerPosition)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    heavyStationSurface,
                    pointerPosition))
            {
                return WorkStationType.HeavyCargo;
            }

            if (RectTransformUtility.RectangleContainsScreenPoint(
                    fragileStationSurface,
                    pointerPosition))
            {
                return WorkStationType.FragileCargo;
            }

            return null;
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
            gameObject.transform.SetParent(cargoPool, false);

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
            ResetStationColors();
        }

        private IEnumerator SnapIntoStation(
            CargoParcelView box,
            WorkStationType station,
            bool completesProcessing)
        {
            box.SetInteraction(false);
            var targetSlot = station == WorkStationType.HeavyCargo
                ? heavyCargoSlot
                : fragileCargoSlot;
            var startPosition = box.RectTransform.position;
            var startScale = box.RectTransform.localScale;
            var targetPosition = targetSlot.position;

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

            box.RectTransform.SetParent(targetSlot, false);
            box.RectTransform.anchoredPosition = Vector2.zero;
            box.RectTransform.localScale = Vector3.one * 0.86f;
            box.RectTransform.localEulerAngles = Vector3.zero;

            if (completesProcessing)
            {
                CompleteProcessing();
            }
        }

        private void CompleteProcessing()
        {
            var heavyResult = results[HeavyParcelId];
            var fragileResult = results[FragileParcelId];
            CargoSessionFlow.Instance.State.CompleteProcessing(
                heavyResult,
                fragileResult);
            completionLabel.text =
                $"Carga pesada: {ResultLabel(heavyResult)}\n" +
                $"Carga frágil: {ResultLabel(fragileResult)}";
            completionPanel.SetActive(true);
            continueButton.interactable = true;
            statusLabel.text = "LAS DOS CAJAS FUERON PROCESADAS";
        }

        private static string ResultLabel(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient => "INEFICIENTE",
                CargoProcessingResult.Damaged => "DAÑADA",
                _ => "NORMAL"
            };
        }

        private static string ResultStatus(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient => "PROCESAMIENTO INEFICIENTE",
                CargoProcessingResult.Damaged => "LA CAJA QUEDÓ DAÑADA",
                _ => "PROCESAMIENTO NORMAL"
            };
        }

        private static Color ResultColor(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient => UiElementFactory.Hex("#F7C843"),
                CargoProcessingResult.Damaged => UiElementFactory.Hex("#EF4444"),
                _ => UiElementFactory.Hex("#65D6AD")
            };
        }

        private Text ResultText(WorkStationType station)
        {
            return station == WorkStationType.HeavyCargo
                ? heavyResultLabel
                : fragileResultLabel;
        }

        private void HighlightStation(WorkStationType station)
        {
            var heavy = heavyStationColor;
            var fragile = fragileStationColor;
            heavy.a = station == WorkStationType.HeavyCargo ? 0.62f : 0.18f;
            fragile.a = station == WorkStationType.FragileCargo ? 0.62f : 0.18f;
            heavyStationBackground.color = heavy;
            fragileStationBackground.color = fragile;
        }

        private void ResetStationColors()
        {
            var heavy = heavyStationColor;
            var fragile = fragileStationColor;
            heavy.a = 0.3f;
            fragile.a = 0.3f;
            heavyStationBackground.color = heavy;
            fragileStationBackground.color = fragile;
        }

        private static void Continue()
        {
            CargoSessionFlow.Instance.GoToPalletAssembly();
        }
    }
}
