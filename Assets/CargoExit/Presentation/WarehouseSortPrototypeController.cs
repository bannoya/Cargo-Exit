using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class WarehouseSortPrototypeController : MonoBehaviour
    {
        private sealed class Pallet
        {
            public string Letter;
            public RectTransform Surface;
            public Image Background;
            public Text Counter;
            public Color Color;
            public int Count;
        }

        private readonly List<WarehouseSortParcelView> boxes = new();
        private readonly Dictionary<string, Pallet> pallets = new();

        private Canvas canvas;
        private RectTransform canvasRect;
        private RectTransform pileSurface;
        private Text statusLabel;
        private Text comboLabel;
        private int combo;
        private bool roundEnding;

        public Transform CanvasTransform => canvas.transform;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera();
            BuildInterface();
            StartRound();
        }

        public void BeginSort(WarehouseSortParcelView box)
        {
            box.RectTransform.localScale = Vector3.one * 1.07f;
            statusLabel.text = $"BUSCÁ EL PALLET {box.DestinationLetter}";
            HighlightDestination(box.DestinationLetter);
        }

        public void ContinueSort(WarehouseSortParcelView box, Vector3 worldTarget)
        {
            var local = (Vector2)canvasRect.InverseTransformPoint(worldTarget);
            var targetPallet = pallets[box.DestinationLetter];
            var palletCenter = (Vector2)canvasRect.InverseTransformPoint(
                targetPallet.Surface.position);
            var distance = Vector2.Distance(local, palletCenter);

            if (distance < 270f)
            {
                var attraction = 1f - Mathf.Clamp01(distance / 270f);
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

        public void EndSort(WarehouseSortParcelView box, Vector2 pointerPosition)
        {
            var correctPallet = pallets[box.DestinationLetter];
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    correctPallet.Surface,
                    pointerPosition))
            {
                combo++;
                UpdateCombo();
                statusLabel.text = combo == 1
                    ? "ENCAJÓ"
                    : $"BIEN ×{combo}";
                ResetPalletColors();
                StartCoroutine(SnapIntoPallet(box, correctPallet));
                return;
            }

            var wrongPallet = pallets.Values.FirstOrDefault(
                pallet => RectTransformUtility.RectangleContainsScreenPoint(
                    pallet.Surface,
                    pointerPosition));

            combo = 0;
            UpdateCombo();
            statusLabel.text = wrongPallet == null
                ? $"La caja {box.DestinationLetter} va en su pallet"
                : $"Ese es {wrongPallet.Letter}; buscá {box.DestinationLetter}";
            HighlightDestination(box.DestinationLetter);
            StartCoroutine(ReturnToPile(box));
        }

        private void BuildInterface()
        {
            var root = new GameObject(
                "Warehouse Sort Interface",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);

            canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasRect = root.GetComponent<RectTransform>();

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var background = PrototypeUi.Panel(
                root.transform,
                "Background",
                PrototypeUi.Hex("#FFF4D6"),
                new Vector2(1080f, 1920f),
                Vector2.zero);
            background.anchorMin = Vector2.zero;
            background.anchorMax = Vector2.one;
            background.sizeDelta = Vector2.zero;

            PrototypeUi.Label(
                root.transform,
                "Brand",
                "BANNOYA'S GAMES",
                24,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(700f, 48f),
                new Vector2(0f, 887f),
                FontStyle.Bold);

            PrototypeUi.Label(
                root.transform,
                "Title",
                "ORDENÁ EL DEPÓSITO",
                54,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                new Vector2(930f, 82f),
                new Vector2(0f, 824f),
                FontStyle.Bold);

            statusLabel = PrototypeUi.Label(
                root.transform,
                "Status",
                string.Empty,
                29,
                PrototypeUi.Hex("#374151"),
                TextAnchor.MiddleCenter,
                new Vector2(860f, 66f),
                new Vector2(-40f, 762f),
                FontStyle.Bold);

            comboLabel = PrototypeUi.Label(
                root.transform,
                "Combo",
                string.Empty,
                25,
                PrototypeUi.Hex("#D97706"),
                TextAnchor.MiddleRight,
                new Vector2(260f, 60f),
                new Vector2(365f, 762f),
                FontStyle.Bold);

            CreatePallet("A", new Vector2(-320f, 620f), new Vector2(280f, 190f));
            CreatePallet("B", new Vector2(0f, 620f), new Vector2(280f, 190f));
            CreatePallet("C", new Vector2(320f, 620f), new Vector2(280f, 190f));
            CreatePallet("D", new Vector2(-400f, 250f), new Vector2(250f, 310f));
            CreatePallet("E", new Vector2(-400f, -145f), new Vector2(250f, 310f));
            CreatePallet("F", new Vector2(400f, 250f), new Vector2(250f, 310f));
            CreatePallet("G", new Vector2(400f, -145f), new Vector2(250f, 310f));

            pileSurface = PrototypeUi.Panel(
                root.transform,
                "Unsorted Boxes",
                new Color(1f, 1f, 1f, 0.48f),
                new Vector2(465f, 955f),
                new Vector2(0f, 70f));

            PrototypeUi.Label(
                pileSurface,
                "PileTitle",
                "CAJAS DESORDENADAS",
                23,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.UpperCenter,
                new Vector2(430f, 60f),
                new Vector2(0f, 432f),
                FontStyle.Bold);

            PrototypeUi.Label(
                root.transform,
                "Hint",
                "Arrastrá cada caja al pallet con la misma letra",
                27,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 70f),
                new Vector2(0f, -790f));

            EnsureEventSystem();
        }

        private void CreatePallet(string letter, Vector2 position, Vector2 size)
        {
            var color = LetterColor(letter);
            var backgroundColor = color;
            backgroundColor.a = 0.24f;

            var surface = PrototypeUi.Panel(
                canvas.transform,
                $"Pallet {letter}",
                backgroundColor,
                size,
                position);

            PrototypeUi.Label(
                surface,
                "Letter",
                letter,
                66,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                new Vector2(150f, 100f),
                new Vector2(0f, 18f),
                FontStyle.Bold);

            var counter = PrototypeUi.Label(
                surface,
                "Counter",
                "0 / 2",
                22,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.LowerCenter,
                new Vector2(180f, 46f),
                new Vector2(0f, -size.y * 0.5f + 30f),
                FontStyle.Bold);

            pallets.Add(letter, new Pallet
            {
                Letter = letter,
                Surface = surface,
                Background = surface.GetComponent<Image>(),
                Counter = counter,
                Color = color,
                Count = 0
            });
        }

        private void StartRound()
        {
            combo = 0;
            roundEnding = false;
            statusLabel.text = "ELEGÍ UNA CAJA";
            UpdateCombo();
            ResetPalletColors();

            foreach (var pallet in pallets.Values)
            {
                pallet.Count = 0;
                pallet.Counter.text = "0 / 2";
            }

            var letters = new[]
            {
                "D", "A", "F", "C", "G", "B", "E",
                "C", "E", "A", "G", "D", "B", "F"
            };
            var positions = new[]
            {
                new Vector2(-88f, 342f),
                new Vector2(90f, 292f),
                new Vector2(-105f, 235f),
                new Vector2(96f, 178f),
                new Vector2(-76f, 120f),
                new Vector2(84f, 62f),
                new Vector2(-100f, 4f),
                new Vector2(102f, -55f),
                new Vector2(-82f, -114f),
                new Vector2(88f, -173f),
                new Vector2(-103f, -232f),
                new Vector2(96f, -290f),
                new Vector2(-72f, -342f),
                new Vector2(76f, -365f)
            };
            var rotations = new[]
            {
                -5f, 4f, -3f, 6f, -4f, 3f, -6f,
                5f, -3f, 4f, -5f, 3f, -4f, 5f
            };

            for (var i = 0; i < letters.Length; i++)
            {
                CreateBox(
                    $"Box {i + 1}",
                    letters[i],
                    positions[i],
                    rotations[i]);
            }
        }

        private void CreateBox(
            string name,
            string destinationLetter,
            Vector2 position,
            float rotation)
        {
            var gameObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(WarehouseSortParcelView));
            gameObject.transform.SetParent(pileSurface, false);

            var view = gameObject.GetComponent<WarehouseSortParcelView>();
            view.Initialize(
                this,
                destinationLetter,
                LetterColor(destinationLetter),
                position,
                rotation);
            boxes.Add(view);
        }

        private IEnumerator ReturnToPile(WarehouseSortParcelView box)
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
            WarehouseSortParcelView box,
            Pallet pallet)
        {
            box.SetInteraction(false);
            var startPosition = box.RectTransform.position;
            var startScale = box.RectTransform.localScale;
            var slot = new Vector2(pallet.Count == 0 ? -58f : 58f, -5f);
            var targetPosition = pallet.Surface.TransformPoint(slot);
            pallet.Count++;
            pallet.Counter.text = $"{pallet.Count} / 2";

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

            if (!roundEnding && pallets.Values.All(item => item.Count == 2))
            {
                roundEnding = true;
                StartCoroutine(CompleteRound());
            }
        }

        private IEnumerator CompleteRound()
        {
            statusLabel.text = "¡DEPÓSITO ORDENADO!";
            foreach (var pallet in pallets.Values)
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

        private IEnumerator PulsePallet(Pallet pallet)
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

        private void HighlightDestination(string letter)
        {
            foreach (var pallet in pallets.Values)
            {
                var color = pallet.Color;
                color.a = pallet.Letter == letter ? 0.56f : 0.16f;
                pallet.Background.color = color;
            }
        }

        private void ResetPalletColors()
        {
            foreach (var pallet in pallets.Values)
            {
                var color = pallet.Color;
                color.a = 0.24f;
                pallet.Background.color = color;
            }
        }

        private void UpdateCombo()
        {
            comboLabel.text = combo > 1 ? $"RACHA ×{combo}" : string.Empty;
        }

        private static Color LetterColor(string letter)
        {
            return letter switch
            {
                "A" => PrototypeUi.Hex("#F7C843"),
                "B" => PrototypeUi.Hex("#65D6AD"),
                "C" => PrototypeUi.Hex("#FF8A65"),
                "D" => PrototypeUi.Hex("#65A9E8"),
                "E" => PrototypeUi.Hex("#B794F4"),
                "F" => PrototypeUi.Hex("#4FD1C5"),
                "G" => PrototypeUi.Hex("#F6AD55"),
                _ => PrototypeUi.Hex("#E5E7EB")
            };
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
            {
                return;
            }

            var cameraObject = new GameObject(
                "Main Camera",
                typeof(Camera),
                typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);

            var gameCamera = cameraObject.GetComponent<Camera>();
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = PrototypeUi.Hex("#FFF4D6");
            gameCamera.orthographic = true;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                new GameObject(
                    "EventSystem",
                    typeof(EventSystem),
                    typeof(StandaloneInputModule));
            }
        }
    }
}

