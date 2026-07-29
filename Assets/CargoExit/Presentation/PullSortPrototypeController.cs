using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class PullSortPrototypeController : MonoBehaviour
    {
        private sealed class Dock
        {
            public RectTransform Surface;
            public Image Background;
            public Text Counter;
            public int Count;
        }

        private readonly List<PullSortParcelView> parcels = new();
        private readonly Dictionary<PullSortDestination, Dock> docks = new();

        private Canvas canvas;
        private RectTransform canvasRect;
        private RectTransform truckSurface;
        private RectTransform gateSurface;
        private Text statusLabel;
        private Text comboLabel;
        private bool gateCrossed;
        private bool roundEnding;
        private int combo;

        public Transform CanvasTransform => canvas.transform;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera();
            BuildInterface();
            StartRound();
        }

        public void BeginPull(PullSortParcelView parcel)
        {
            gateCrossed = false;
            parcel.RectTransform.localScale = Vector3.one * 1.06f;
            statusLabel.text = "BAJALA POR EL CENTRO";
            SetDockGlow(false);
        }

        public void ContinuePull(PullSortParcelView parcel, Vector3 worldTarget)
        {
            var local = (Vector2)canvasRect.InverseTransformPoint(worldTarget);

            if (!gateCrossed)
            {
                var approach = Mathf.InverseLerp(320f, -230f, local.y);
                var magneticStrength = Mathf.SmoothStep(0f, 0.72f, approach);
                local.x = Mathf.Lerp(local.x, 0f, magneticStrength);

                if (local.y <= -205f && Mathf.Abs(local.x) <= 155f)
                {
                    gateCrossed = true;
                    statusLabel.text = "AHORA ACOMODALA";
                    SetDockGlow(true);
                    StartCoroutine(GatePulse());
                }
            }

            parcel.RectTransform.anchoredPosition = local;
        }

        public void EndPull(PullSortParcelView parcel, Vector2 pointerPosition)
        {
            if (!gateCrossed)
            {
                combo = 0;
                UpdateCombo();
                statusLabel.text = "Primero pasá la caja por la salida";
                SetDockGlow(false);
                StartCoroutine(ReturnHome(parcel));
                return;
            }

            var correctDock = docks[parcel.Destination];
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    correctDock.Surface,
                    pointerPosition))
            {
                combo++;
                UpdateCombo();
                statusLabel.text = combo == 1
                    ? "ENCAJÓ PERFECTO"
                    : $"SUAVE ×{combo}";
                SetDockGlow(false);
                StartCoroutine(SnapIntoDock(parcel, correctDock));
                return;
            }

            combo = 0;
            UpdateCombo();
            statusLabel.text = "Llevala al símbolo que tiene la caja";
            SetDockGlow(false);
            StartCoroutine(ReturnHome(parcel));
        }

        private void BuildInterface()
        {
            var root = new GameObject(
                "Pull Sort Interface",
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
                25,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(700f, 50f),
                new Vector2(0f, 884f),
                FontStyle.Bold);

            PrototypeUi.Label(
                root.transform,
                "Title",
                "BAJÁ Y ACOMODÁ",
                58,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 90f),
                new Vector2(0f, 814f),
                FontStyle.Bold);

            statusLabel = PrototypeUi.Label(
                root.transform,
                "Status",
                string.Empty,
                30,
                PrototypeUi.Hex("#374151"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 70f),
                new Vector2(0f, 742f),
                FontStyle.Bold);

            comboLabel = PrototypeUi.Label(
                root.transform,
                "Combo",
                string.Empty,
                28,
                PrototypeUi.Hex("#D97706"),
                TextAnchor.MiddleRight,
                new Vector2(300f, 60f),
                new Vector2(325f, 742f),
                FontStyle.Bold);

            var truckFrame = PrototypeUi.Panel(
                root.transform,
                "Truck",
                PrototypeUi.Hex("#172238"),
                new Vector2(900f, 930f),
                new Vector2(0f, 232f));

            truckSurface = PrototypeUi.Panel(
                truckFrame,
                "TruckInterior",
                PrototypeUi.Hex("#33415C"),
                new Vector2(850f, 880f),
                Vector2.zero);

            BuildTruckFloor();

            PrototypeUi.Label(
                root.transform,
                "PullHint",
                "DESLIZÁ HACIA ABAJO",
                25,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(500f, 50f),
                new Vector2(0f, -260f),
                FontStyle.Bold);

            gateSurface = PrototypeUi.Panel(
                root.transform,
                "ExitGate",
                PrototypeUi.Hex("#F7C843"),
                new Vector2(270f, 120f),
                new Vector2(0f, -342f));
            PrototypeUi.Label(
                gateSurface,
                "GateLabel",
                "▼  SALIDA  ▼",
                28,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                gateSurface.sizeDelta,
                Vector2.zero,
                FontStyle.Bold);

            CreateDock(
                PullSortDestination.Left,
                new Vector2(-300f, -628f),
                PrototypeUi.Hex("#F7C843"),
                "●  A");
            CreateDock(
                PullSortDestination.Right,
                new Vector2(300f, -628f),
                PrototypeUi.Hex("#65D6AD"),
                "▲  B");

            PrototypeUi.Label(
                root.transform,
                "GestureHint",
                "Un solo gesto: bajá al centro y doblá hacia un costado",
                25,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 70f),
                new Vector2(0f, -850f));

            EnsureEventSystem();
        }

        private void BuildTruckFloor()
        {
            for (var row = 0; row < 4; row++)
            {
                var line = PrototypeUi.Panel(
                    truckSurface,
                    $"FloorLine_{row}",
                    new Color(1f, 1f, 1f, 0.07f),
                    new Vector2(810f, 3f),
                    new Vector2(0f, -330f + row * 220f));
                line.GetComponent<Image>().raycastTarget = false;
            }
        }

        private void CreateDock(
            PullSortDestination destination,
            Vector2 position,
            Color color,
            string symbol)
        {
            var surface = PrototypeUi.Panel(
                canvas.transform,
                destination == PullSortDestination.Left ? "Dock A" : "Dock B",
                new Color(color.r, color.g, color.b, 0.28f),
                new Vector2(430f, 330f),
                position);
            var image = surface.GetComponent<Image>();

            PrototypeUi.Label(
                surface,
                "Symbol",
                symbol,
                48,
                PrototypeUi.Hex("#172238"),
                TextAnchor.UpperCenter,
                new Vector2(360f, 90f),
                new Vector2(0f, 105f),
                FontStyle.Bold);

            var counter = PrototypeUi.Label(
                surface,
                "Counter",
                "0 / 3",
                25,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.LowerCenter,
                new Vector2(300f, 55f),
                new Vector2(0f, -125f),
                FontStyle.Bold);

            docks.Add(destination, new Dock
            {
                Surface = surface,
                Background = image,
                Counter = counter,
                Count = 0
            });
        }

        private void StartRound()
        {
            combo = 0;
            roundEnding = false;
            UpdateCombo();
            statusLabel.text = "ELEGÍ UNA CAJA";

            foreach (var dock in docks.Values)
            {
                dock.Count = 0;
                dock.Counter.text = "0 / 3";
            }

            CreateParcel("Box 1", PullSortDestination.Left, new Vector2(-260f, 245f));
            CreateParcel("Box 2", PullSortDestination.Right, new Vector2(0f, 245f));
            CreateParcel("Box 3", PullSortDestination.Left, new Vector2(260f, 245f));
            CreateParcel("Box 4", PullSortDestination.Right, new Vector2(-260f, -35f));
            CreateParcel("Box 5", PullSortDestination.Right, new Vector2(0f, -35f));
            CreateParcel("Box 6", PullSortDestination.Left, new Vector2(260f, -35f));
        }

        private void CreateParcel(
            string name,
            PullSortDestination destination,
            Vector2 position)
        {
            var gameObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(PullSortParcelView));
            gameObject.transform.SetParent(truckSurface, false);

            var isLeft = destination == PullSortDestination.Left;
            var view = gameObject.GetComponent<PullSortParcelView>();
            view.Initialize(
                this,
                destination,
                isLeft ? PrototypeUi.Hex("#F7C843") : PrototypeUi.Hex("#65D6AD"),
                isLeft ? "●  A" : "▲  B",
                position);
            parcels.Add(view);
        }

        private IEnumerator ReturnHome(PullSortParcelView parcel)
        {
            parcel.SetInteraction(false);
            var startPosition = parcel.RectTransform.anchoredPosition;
            var startScale = parcel.RectTransform.localScale;
            var homeWorld = parcel.RectTransform.parent.TransformPoint(Vector3.zero);
            parcel.RestoreHome();
            var targetWorld = parcel.RectTransform.position;
            parcel.RectTransform.SetParent(canvas.transform, true);
            parcel.RectTransform.position = homeWorld;
            parcel.RectTransform.anchoredPosition = startPosition;
            parcel.RectTransform.localScale = startScale;

            const float duration = 0.28f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 3f);
                parcel.RectTransform.position = Vector3.Lerp(
                    canvasRect.TransformPoint(startPosition),
                    targetWorld,
                    eased);
                parcel.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    Vector3.one,
                    eased);
                yield return null;
            }

            parcel.RestoreHome();
        }

        private IEnumerator SnapIntoDock(PullSortParcelView parcel, Dock dock)
        {
            parcel.SetInteraction(false);
            var startPosition = parcel.RectTransform.position;
            var startScale = parcel.RectTransform.localScale;
            var slot = DockSlot(dock.Count);
            var targetPosition = dock.Surface.TransformPoint(slot);
            dock.Count++;
            dock.Counter.text = $"{dock.Count} / 3";

            const float duration = 0.24f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var overshoot = 1f + 2.2f * Mathf.Pow(t - 1f, 3f) +
                                1.2f * Mathf.Pow(t - 1f, 2f);
                parcel.RectTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    overshoot);
                parcel.RectTransform.localScale = Vector3.Lerp(
                    startScale,
                    Vector3.one * 0.67f,
                    t);
                yield return null;
            }

            parcel.RectTransform.SetParent(dock.Surface, false);
            parcel.RectTransform.anchoredPosition = slot;
            parcel.RectTransform.localScale = Vector3.one * 0.67f;

            if (!roundEnding && AllParcelsSorted())
            {
                roundEnding = true;
                StartCoroutine(CompleteRound());
            }
        }

        private IEnumerator CompleteRound()
        {
            statusLabel.text = "¡TODO EN SU LUGAR!";
            yield return new WaitForSecondsRealtime(1.35f);

            foreach (var parcel in parcels)
            {
                Destroy(parcel.gameObject);
            }

            parcels.Clear();
            yield return null;
            StartRound();
        }

        private IEnumerator GatePulse()
        {
            var startScale = gateSurface.localScale;
            for (var elapsed = 0f; elapsed < 0.18f; elapsed += Time.unscaledDeltaTime)
            {
                var normalized = elapsed / 0.18f;
                var pulse = 1f + Mathf.Sin(normalized * Mathf.PI) * 0.1f;
                gateSurface.localScale = startScale * pulse;
                yield return null;
            }

            gateSurface.localScale = startScale;
        }

        private bool AllParcelsSorted()
        {
            foreach (var dock in docks.Values)
            {
                if (dock.Count < 3)
                {
                    return false;
                }
            }

            return true;
        }

        private static Vector2 DockSlot(int index)
        {
            var column = index % 2;
            var row = index / 2;
            return new Vector2(
                column == 0 ? -82f : 82f,
                row == 0 ? -28f : 82f);
        }

        private void SetDockGlow(bool active)
        {
            foreach (var pair in docks)
            {
                var color = pair.Key == PullSortDestination.Left
                    ? PrototypeUi.Hex("#F7C843")
                    : PrototypeUi.Hex("#65D6AD");
                color.a = active ? 0.58f : 0.28f;
                pair.Value.Background.color = color;
            }
        }

        private void UpdateCombo()
        {
            comboLabel.text = combo > 1 ? $"RACHA ×{combo}" : string.Empty;
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

