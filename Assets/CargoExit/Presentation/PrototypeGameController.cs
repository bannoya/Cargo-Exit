using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BannoyasGames.CargoExit.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class PrototypeGameController : MonoBehaviour
    {
        private const int BoardWidth = 4;
        private const int BoardHeight = 5;
        private const float CellSize = 112f;

        private readonly List<ParcelView> parcels = new();
        private readonly Dictionary<string, Vector2> trayPositions = new();
        private Canvas canvas;
        private RectTransform boardSurface;
        private RectTransform traySurface;
        private Text statusLabel;
        private Button deliveryButton;
        private CargoBoard board;
        private bool deliveryRunning;

        public Transform CanvasTransform => canvas.transform;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
            EnsureCamera();
            BuildInterface();
            CreatePrototypeParcels();
            ResetLevel();
        }

        public void OnParcelDragStarted(ParcelView parcel)
        {
            ClearHighlights();
            statusLabel.text = "Soltalo dentro de la camioneta";
        }

        public void OnParcelDragEnded(ParcelView parcel)
        {
            var localPosition = boardSurface.InverseTransformPoint(
                parcel.RectTransform.position);
            var origin = new GridPosition(
                Mathf.RoundToInt(localPosition.x / CellSize),
                Mathf.RoundToInt(localPosition.y / CellSize));
            var candidate = new ParcelPlacement(
                parcel.Definition,
                origin,
                parcel.Rotation);
            var result = board.TryPlace(candidate);

            if (result.Succeeded)
            {
                parcel.PutOnBoard(boardSurface, origin);
                statusLabel.text = "Tocá un paquete para rotarlo";
            }
            else
            {
                parcel.RestoreDragSource();
                statusLabel.text = result.Failure == PlacementFailure.Overlap
                    ? "Ese lugar ya está ocupado"
                    : "El paquete debe quedar dentro de la camioneta";
            }

            RefreshDeliveryButton();
        }

        public void RotateParcel(ParcelView parcel)
        {
            if (deliveryRunning)
            {
                return;
            }

            ClearHighlights();
            var nextRotation = (QuarterTurn)(((int)parcel.Rotation + 1) % 4);

            if (!parcel.IsPlaced)
            {
                parcel.SetRotation(nextRotation);
                statusLabel.text = "Rotado. Ahora arrastralo a la camioneta";
                return;
            }

            var candidate = new ParcelPlacement(
                parcel.Definition,
                parcel.BoardOrigin,
                nextRotation);
            var result = board.TryPlace(candidate);

            if (result.Succeeded)
            {
                parcel.SetRotation(nextRotation);
                parcel.PutOnBoard(boardSurface, parcel.BoardOrigin);
                statusLabel.text = "Paquete rotado";
            }
            else
            {
                statusLabel.text = "No hay espacio para rotarlo ahí";
            }
        }

        private void BuildInterface()
        {
            var root = new GameObject(
                "Interface",
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);

            canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

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
                26,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(800f, 60f),
                new Vector2(0f, 865f),
                FontStyle.Bold);

            PrototypeUi.Label(
                root.transform,
                "Title",
                "CARGO EXIT",
                70,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 100f),
                new Vector2(0f, 790f),
                FontStyle.Bold);

            statusLabel = PrototypeUi.Label(
                root.transform,
                "Status",
                string.Empty,
                32,
                PrototypeUi.Hex("#374151"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 100f),
                new Vector2(0f, 690f));

            var frameSize = new Vector2(
                BoardWidth * CellSize + 28f,
                BoardHeight * CellSize + 28f);
            var frame = PrototypeUi.Panel(
                root.transform,
                "Truck",
                PrototypeUi.Hex("#172238"),
                frameSize,
                new Vector2(0f, 310f));

            boardSurface = PrototypeUi.Panel(
                frame,
                "CargoBoard",
                Color.clear,
                new Vector2(BoardWidth * CellSize, BoardHeight * CellSize),
                Vector2.zero);
            boardSurface.anchorMin = new Vector2(0.5f, 0.5f);
            boardSurface.anchorMax = new Vector2(0.5f, 0.5f);
            boardSurface.pivot = Vector2.zero;
            boardSurface.anchoredPosition = new Vector2(
                -BoardWidth * CellSize * 0.5f,
                -BoardHeight * CellSize * 0.5f);

            BuildGrid();

            PrototypeUi.Label(
                root.transform,
                "Door",
                "▼  PUERTA DE CARGA  ▼",
                28,
                PrototypeUi.Hex("#172238"),
                TextAnchor.MiddleCenter,
                new Vector2(600f, 60f),
                new Vector2(0f, -2f),
                FontStyle.Bold);

            traySurface = PrototypeUi.Panel(
                root.transform,
                "LoadingArea",
                new Color(1f, 1f, 1f, 0.42f),
                new Vector2(940f, 280f),
                new Vector2(0f, -250f));

            PrototypeUi.Label(
                traySurface,
                "Hint",
                "PAQUETES",
                24,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(300f, 50f),
                new Vector2(0f, 105f),
                FontStyle.Bold);

            deliveryButton = PrototypeUi.ActionButton(
                root.transform,
                "Deliver",
                "ENTREGAR",
                PrototypeUi.Hex("#F7C843"),
                PrototypeUi.Hex("#172238"),
                new Vector2(430f, 110f),
                new Vector2(-235f, -520f),
                StartDelivery);

            PrototypeUi.ActionButton(
                root.transform,
                "Reset",
                "REINICIAR",
                PrototypeUi.Hex("#E6E8ED"),
                PrototypeUi.Hex("#172238"),
                new Vector2(430f, 110f),
                new Vector2(235f, -520f),
                ResetLevel);

            PrototypeUi.Label(
                root.transform,
                "Controls",
                "Arrastrá para cargar  •  Tocá para rotar",
                28,
                PrototypeUi.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 70f),
                new Vector2(0f, -620f));

            EnsureEventSystem();
        }

        private void BuildGrid()
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                for (var x = 0; x < BoardWidth; x++)
                {
                    var cell = new GameObject(
                        $"Grid_{x}_{y}",
                        typeof(RectTransform),
                        typeof(Image));
                    var rect = cell.GetComponent<RectTransform>();
                    rect.SetParent(boardSurface, false);
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.zero;
                    rect.pivot = Vector2.zero;
                    rect.sizeDelta = new Vector2(CellSize - 6f, CellSize - 6f);
                    rect.anchoredPosition = new Vector2(
                        x * CellSize + 3f,
                        y * CellSize + 3f);
                    cell.GetComponent<Image>().color = PrototypeUi.Hex("#33415C");
                }
            }
        }

        private void CreatePrototypeParcels()
        {
            CreateParcel(
                "P1",
                1,
                new[]
                {
                    new GridPosition(0, 0),
                    new GridPosition(1, 0)
                },
                PrototypeUi.Hex("#F7C843"),
                new Vector2(-290f, -35f));

            CreateParcel(
                "P2",
                2,
                new[]
                {
                    new GridPosition(0, 0),
                    new GridPosition(0, 1),
                    new GridPosition(1, 0)
                },
                PrototypeUi.Hex("#FF8A65"),
                new Vector2(-30f, -45f));

            CreateParcel(
                "P3",
                3,
                new[]
                {
                    new GridPosition(0, 0),
                    new GridPosition(1, 0)
                },
                PrototypeUi.Hex("#65D6AD"),
                new Vector2(260f, -35f));
        }

        private void CreateParcel(
            string id,
            int order,
            IEnumerable<GridPosition> cells,
            Color color,
            Vector2 trayPosition)
        {
            var gameObject = new GameObject(id, typeof(RectTransform), typeof(ParcelView));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.pivot = Vector2.zero;

            var definition = new ParcelDefinition(id, order, new ParcelShape(cells));
            var view = gameObject.GetComponent<ParcelView>();
            view.Initialize(this, definition, color, CellSize);
            parcels.Add(view);
            trayPositions.Add(id, trayPosition);
        }

        private void StartDelivery()
        {
            if (deliveryRunning || parcels.Any(parcel => !parcel.IsPlaced))
            {
                statusLabel.text = "Primero cargá todos los paquetes";
                return;
            }

            ClearHighlights();
            var result = DeliverySimulator.Evaluate(board);
            if (!result.Succeeded)
            {
                var failing = parcels.Single(
                    parcel => parcel.Definition.Id == result.FailingParcelId);
                failing.SetHighlight(true);

                foreach (var blockerId in result.BlockingParcelIds)
                {
                    parcels.Single(parcel => parcel.Definition.Id == blockerId)
                        .SetHighlight(true);
                }

                statusLabel.text =
                    $"El paquete {failing.Definition.DeliveryOrder} no puede llegar a la puerta";
                return;
            }

            StartCoroutine(PlaySuccessfulDelivery());
        }

        private IEnumerator PlaySuccessfulDelivery()
        {
            deliveryRunning = true;
            SetParcelInteraction(false);
            deliveryButton.interactable = false;
            statusLabel.text = "¡Ruta despejada!";

            foreach (var parcel in parcels.OrderBy(item => item.Definition.DeliveryOrder))
            {
                var start = parcel.RectTransform.anchoredPosition;
                var end = start + new Vector2(0f, -BoardHeight * CellSize - 180f);
                const float duration = 0.32f;
                var elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    var t = Mathf.Clamp01(elapsed / duration);
                    t = 1f - Mathf.Pow(1f - t, 3f);
                    parcel.RectTransform.anchoredPosition = Vector2.Lerp(start, end, t);
                    yield return null;
                }

                parcel.gameObject.SetActive(false);
                yield return new WaitForSecondsRealtime(0.12f);
            }

            statusLabel.text = "¡ENTREGA PERFECTA!";
        }

        private void ResetLevel()
        {
            StopAllCoroutines();
            deliveryRunning = false;
            board = new CargoBoard(BoardWidth, BoardHeight);

            foreach (var parcel in parcels)
            {
                parcel.gameObject.SetActive(true);
                parcel.ResetVisualState();
                parcel.SetInteraction(true);
                parcel.PutInTray(traySurface, trayPositions[parcel.Definition.Id]);
            }

            statusLabel.text = "Cargá todo y dejá libre el orden 1 → 2 → 3";
            RefreshDeliveryButton();
        }

        private void RefreshDeliveryButton()
        {
            deliveryButton.interactable =
                !deliveryRunning && parcels.All(parcel => parcel.IsPlaced);
        }

        private void SetParcelInteraction(bool enabled)
        {
            foreach (var parcel in parcels)
            {
                parcel.SetInteraction(enabled);
            }
        }

        private void ClearHighlights()
        {
            foreach (var parcel in parcels)
            {
                parcel.SetHighlight(false);
            }
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule));
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
            gameCamera.orthographicSize = 5f;
        }
    }
}
