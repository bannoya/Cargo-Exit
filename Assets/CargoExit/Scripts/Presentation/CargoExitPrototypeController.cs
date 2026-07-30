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
        [SerializeField] private TMP_FontAsset regularFont;
        [SerializeField] private TMP_FontAsset boldFont;

        private readonly List<CargoParcelView> boxes = new();
        private CargoExitPrototypeView view;
        private CargoSortSession session;
        private int combo;
        private bool roundEnding;

        public Transform CanvasTransform => view.Canvas.transform;

        private void Awake()
        {
            Application.targetFrameRate = 60;
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

            StartRound();
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

        public void BeginSort(CargoParcelView box)
        {
            box.RectTransform.localScale = Vector3.one * 1.07f;
            view.StatusLabel.text = $"BUSCÁ EL PALLET {box.Destination}";
            HighlightDestination(box.Destination);
        }

        public void ContinueSort(CargoParcelView box, Vector3 worldTarget)
        {
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
            ResetPalletColors();
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
