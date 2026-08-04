using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    internal static class CargoExitPrototypeViewFactory
    {
        private const string InterfaceName = "Game UI";

        private static readonly CargoDestination[] Destinations =
        {
            CargoDestination.A,
            CargoDestination.B,
            CargoDestination.C,
            CargoDestination.D,
            CargoDestination.E,
            CargoDestination.F,
            CargoDestination.G
        };

        public static CargoExitPrototypeView Create(Transform owner)
        {
            var root = new GameObject(
                InterfaceName,
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            root.transform.SetParent(owner, false);

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = true;
            var canvasRect = root.GetComponent<RectTransform>();

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(540f, 960f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var background = UiElementFactory.Panel(
                root.transform,
                "Background",
                UiElementFactory.Hex("#FFF4D6"),
                new Vector2(540f, 960f),
                Vector2.zero);
            background.anchorMin = Vector2.zero;
            background.anchorMax = Vector2.one;
            background.sizeDelta = Vector2.zero;

            UiElementFactory.Label(
                root.transform,
                "Brand",
                "BANNOYA'S GAMES",
                CargoExitTypography.Brand,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Center,
                new Vector2(350f, 30f),
                new Vector2(0f, 443f),
                FontStyles.Bold);

            UiElementFactory.Label(
                root.transform,
                "Title",
                "ORDENÁ EL DEPÓSITO",
                CargoExitTypography.Title,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                new Vector2(480f, 46f),
                new Vector2(0f, 410f),
                FontStyles.Bold);

            var statusLabel = UiElementFactory.Label(
                root.transform,
                "Status",
                string.Empty,
                CargoExitTypography.Status,
                UiElementFactory.Hex("#374151"),
                TextAlignmentOptions.Center,
                new Vector2(390f, 36f),
                new Vector2(-20f, 373f),
                FontStyles.Bold);

            var comboLabel = UiElementFactory.Label(
                root.transform,
                "Combo",
                string.Empty,
                CargoExitTypography.Combo,
                UiElementFactory.Hex("#D97706"),
                TextAlignmentOptions.Right,
                new Vector2(130f, 32f),
                new Vector2(182f, 373f),
                FontStyles.Bold);

            var pallets = new Dictionary<CargoDestination, CargoPalletView>
            {
                [CargoDestination.A] = CreatePallet(
                    canvas.transform,
                    CargoDestination.A,
                    new Vector2(-160f, 305f),
                    new Vector2(140f, 95f)),
                [CargoDestination.B] = CreatePallet(
                    canvas.transform,
                    CargoDestination.B,
                    new Vector2(0f, 305f),
                    new Vector2(140f, 95f)),
                [CargoDestination.C] = CreatePallet(
                    canvas.transform,
                    CargoDestination.C,
                    new Vector2(160f, 305f),
                    new Vector2(140f, 95f)),
                [CargoDestination.D] = CreatePallet(
                    canvas.transform,
                    CargoDestination.D,
                    new Vector2(-205f, 120f),
                    new Vector2(125f, 155f)),
                [CargoDestination.E] = CreatePallet(
                    canvas.transform,
                    CargoDestination.E,
                    new Vector2(-205f, -75f),
                    new Vector2(125f, 155f)),
                [CargoDestination.F] = CreatePallet(
                    canvas.transform,
                    CargoDestination.F,
                    new Vector2(205f, 120f),
                    new Vector2(125f, 155f)),
                [CargoDestination.G] = CreatePallet(
                    canvas.transform,
                    CargoDestination.G,
                    new Vector2(205f, -75f),
                    new Vector2(125f, 155f))
            };

            var pileSurface = UiElementFactory.Panel(
                root.transform,
                "Unsorted Boxes",
                new Color(1f, 1f, 1f, 0.48f),
                new Vector2(230f, 480f),
                new Vector2(0f, 35f));

            UiElementFactory.Label(
                pileSurface,
                "PileTitle",
                "CAJAS DESORDENADAS",
                CargoExitTypography.PileTitle,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Top,
                new Vector2(215f, 36f),
                new Vector2(0f, 214f),
                FontStyles.Bold);

            UiElementFactory.Label(
                root.transform,
                "Hint",
                "Arrastrá cada caja al pallet con la misma letra",
                CargoExitTypography.Hint,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Center,
                new Vector2(490f, 50f),
                new Vector2(0f, -415f));

            EnsureEventSystem(owner);
            return new CargoExitPrototypeView(
                canvas,
                canvasRect,
                pileSurface,
                statusLabel,
                comboLabel,
                pallets);
        }

        public static bool TryBind(
            Transform owner,
            out CargoExitPrototypeView view)
        {
            var interfaceTransform = owner.Find(InterfaceName);
            var canvas = interfaceTransform?.GetComponent<Canvas>();
            var canvasRect = interfaceTransform?.GetComponent<RectTransform>();
            var statusLabel =
                interfaceTransform?.Find("Status")?.GetComponent<TMP_Text>();
            var comboLabel =
                interfaceTransform?.Find("Combo")?.GetComponent<TMP_Text>();
            var pileSurface =
                interfaceTransform?.Find("Unsorted Boxes") as RectTransform;

            if (canvas == null ||
                canvasRect == null ||
                statusLabel == null ||
                comboLabel == null ||
                pileSurface == null)
            {
                view = null;
                return false;
            }

            var pallets = new Dictionary<CargoDestination, CargoPalletView>();
            foreach (var destination in Destinations)
            {
                var surface =
                    interfaceTransform.Find($"Pallet {destination}") as RectTransform;
                var counter = surface?.Find("Counter")?.GetComponent<TMP_Text>();
                var background = surface?.GetComponent<Image>();
                if (surface == null || counter == null || background == null)
                {
                    view = null;
                    return false;
                }

                pallets.Add(
                    destination,
                    new CargoPalletView(
                        destination,
                        surface,
                        background,
                        counter,
                        DestinationColor(destination)));
            }

            EnsureEventSystem(owner);
            view = new CargoExitPrototypeView(
                canvas,
                canvasRect,
                pileSurface,
                statusLabel,
                comboLabel,
                pallets);
            return true;
        }

        public static CargoExitWorkTestView CreateWorkTest(
            Transform parent,
            UnityAction confirmAssignment,
            UnityAction correctAssignment,
            UnityAction confirmImperfectAssignment)
        {
            var root = UiElementFactory.Panel(
                parent,
                "Work Test",
                UiElementFactory.Hex("#FFF4D6"),
                new Vector2(540f, 960f),
                Vector2.zero);
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.sizeDelta = Vector2.zero;
            root.SetAsLastSibling();

            var safeArea = CreateSafeArea(root);

            UiElementFactory.Label(
                safeArea,
                "Brand",
                "BANNOYA'S GAMES",
                CargoExitTypography.Brand,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Center,
                new Vector2(350f, 30f),
                new Vector2(0f, 432f),
                FontStyles.Bold);

            var titleLabel = UiElementFactory.Label(
                safeArea,
                "Title",
                "ASIGNÁ EL EQUIPO",
                CargoExitTypography.Title,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                new Vector2(480f, 48f),
                new Vector2(0f, 392f),
                FontStyles.Bold);

            var statusLabel = UiElementFactory.Label(
                safeArea,
                "Status",
                "OCUPÁ LAS DOS ESTACIONES",
                CargoExitTypography.Status,
                UiElementFactory.Hex("#374151"),
                TextAlignmentOptions.Center,
                new Vector2(480f, 42f),
                new Vector2(0f, 348f),
                FontStyles.Bold);

            var stations = new Dictionary<WorkStationType, CargoWorkStationView>
            {
                [WorkStationType.HeavyCargo] = CreateWorkStation(
                    safeArea,
                    WorkStationType.HeavyCargo,
                    "CARGA PESADA",
                    "Recomendado: FUERZA",
                    new Vector2(-125f, 135f),
                    UiElementFactory.Hex("#F7C843")),
                [WorkStationType.FragileCargo] = CreateWorkStation(
                    safeArea,
                    WorkStationType.FragileCargo,
                    "CARGA FRÁGIL",
                    "Recomendado: CUIDADO",
                    new Vector2(125f, 135f),
                    UiElementFactory.Hex("#FF8A65"))
            };

            var employeePool = UiElementFactory.Panel(
                safeArea,
                "Employee Pool",
                new Color(1f, 1f, 1f, 0.62f),
                new Vector2(490f, 190f),
                new Vector2(0f, -185f));
            UiElementFactory.Label(
                employeePool,
                "Pool Title",
                "EMPLEADOS DISPONIBLES",
                CargoExitTypography.PileTitle,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Top,
                new Vector2(450f, 32f),
                new Vector2(0f, 78f),
                FontStyles.Bold);

            var hintLabel = UiElementFactory.Label(
                safeArea,
                "Hint",
                "Arrastrá cada empleado a una estación",
                CargoExitTypography.Hint,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Center,
                new Vector2(490f, 42f),
                new Vector2(0f, -310f));

            var confirmButton = UiElementFactory.ActionButton(
                safeArea,
                "Confirm Assignment",
                "CONFIRMAR ASIGNACIÓN",
                UiElementFactory.Hex("#172238"),
                Color.white,
                new Vector2(390f, 68f),
                new Vector2(0f, -375f),
                confirmAssignment);
            confirmButton.interactable = false;

            var warningPanel = UiElementFactory.Panel(
                safeArea,
                "Assignment Warning",
                UiElementFactory.Hex("#FFF8E8"),
                new Vector2(480f, 510f),
                new Vector2(0f, -10f));
            warningPanel.SetAsLastSibling();
            UiElementFactory.Label(
                warningPanel,
                "Warning Title",
                "ASIGNACIÓN IMPERFECTA",
                28f,
                UiElementFactory.Hex("#B45309"),
                TextAlignmentOptions.Center,
                new Vector2(430f, 50f),
                new Vector2(0f, 195f),
                FontStyles.Bold);
            var warningLabel = UiElementFactory.Label(
                warningPanel,
                "Warning Text",
                string.Empty,
                CargoExitTypography.Status,
                UiElementFactory.Hex("#374151"),
                TextAlignmentOptions.Center,
                new Vector2(420f, 190f),
                new Vector2(0f, 65f));
            UiElementFactory.ActionButton(
                warningPanel,
                "Correct Assignment",
                "CORREGIR",
                UiElementFactory.Hex("#E5E7EB"),
                UiElementFactory.Hex("#172238"),
                new Vector2(330f, 64f),
                new Vector2(0f, -82f),
                correctAssignment);
            UiElementFactory.ActionButton(
                warningPanel,
                "Confirm Anyway",
                "CONFIRMAR IGUAL",
                UiElementFactory.Hex("#B45309"),
                Color.white,
                new Vector2(330f, 64f),
                new Vector2(0f, -165f),
                confirmImperfectAssignment);
            warningPanel.gameObject.SetActive(false);

            var processingPile = UiElementFactory.Panel(
                safeArea,
                "Processing Cargo",
                new Color(1f, 1f, 1f, 0.62f),
                new Vector2(490f, 215f),
                new Vector2(0f, -190f));
            UiElementFactory.Label(
                processingPile,
                "PileTitle",
                "CAJAS PARA PROCESAR",
                CargoExitTypography.PileTitle,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Top,
                new Vector2(450f, 34f),
                new Vector2(0f, 88f),
                FontStyles.Bold);
            processingPile.gameObject.SetActive(false);

            var finalPanel = UiElementFactory.Panel(
                safeArea,
                "Test Complete",
                UiElementFactory.Hex("#F0FFF4"),
                new Vector2(470f, 255f),
                new Vector2(0f, -175f));
            var finalLabel = UiElementFactory.Label(
                finalPanel,
                "Final Text",
                string.Empty,
                CargoExitTypography.Status,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                new Vector2(430f, 215f),
                Vector2.zero,
                FontStyles.Bold);
            finalPanel.gameObject.SetActive(false);

            return new CargoExitWorkTestView(
                root,
                titleLabel,
                statusLabel,
                hintLabel,
                employeePool,
                confirmButton,
                warningPanel,
                warningLabel,
                processingPile,
                finalPanel,
                finalLabel,
                stations);
        }

        public static CargoEmployeeView CreateEmployee(
            Transform parent,
            CargoExitPrototypeController controller,
            string employeeId,
            string title,
            string skillLabel,
            Color color,
            Vector2 position)
        {
            var rect = UiElementFactory.Panel(
                parent,
                employeeId,
                color,
                new Vector2(190f, 105f),
                position);
            var employee = rect.gameObject.AddComponent<CargoEmployeeView>();

            UiElementFactory.Label(
                rect,
                "Employee Name",
                title,
                24f,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                new Vector2(180f, 40f),
                new Vector2(0f, 17f),
                FontStyles.Bold).raycastTarget = false;
            UiElementFactory.Label(
                rect,
                "Employee Skill",
                skillLabel,
                20f,
                UiElementFactory.Hex("#374151"),
                TextAlignmentOptions.Center,
                new Vector2(180f, 34f),
                new Vector2(0f, -24f)).raycastTarget = false;

            employee.Initialize(controller, employeeId, position);
            return employee;
        }

        public static void EnsureCamera(Transform owner)
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
            cameraObject.transform.SetParent(owner, false);
            cameraObject.transform.localPosition = new Vector3(0f, 0f, -10f);

            var gameCamera = cameraObject.GetComponent<Camera>();
            gameCamera.clearFlags = CameraClearFlags.SolidColor;
            gameCamera.backgroundColor = UiElementFactory.Hex("#FFF4D6");
            gameCamera.orthographic = true;
        }

        public static Color DestinationColor(CargoDestination destination)
        {
            return destination switch
            {
                CargoDestination.A => UiElementFactory.Hex("#F7C843"),
                CargoDestination.B => UiElementFactory.Hex("#65D6AD"),
                CargoDestination.C => UiElementFactory.Hex("#FF8A65"),
                CargoDestination.D => UiElementFactory.Hex("#65A9E8"),
                CargoDestination.E => UiElementFactory.Hex("#B794F4"),
                CargoDestination.F => UiElementFactory.Hex("#4FD1C5"),
                CargoDestination.G => UiElementFactory.Hex("#F6AD55"),
                _ => UiElementFactory.Hex("#E5E7EB")
            };
        }

        private static CargoPalletView CreatePallet(
            Transform parent,
            CargoDestination destination,
            Vector2 position,
            Vector2 size)
        {
            var color = DestinationColor(destination);
            var backgroundColor = color;
            backgroundColor.a = 0.24f;

            var surface = UiElementFactory.Panel(
                parent,
                $"Pallet {destination}",
                backgroundColor,
                size,
                position);

            UiElementFactory.Label(
                surface,
                "Letter",
                destination.ToString(),
                CargoExitTypography.PalletLetter,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                new Vector2(75f, 55f),
                new Vector2(0f, 10f),
                FontStyles.Bold);

            var counter = UiElementFactory.Label(
                surface,
                "Counter",
                "0 / 2",
                CargoExitTypography.PalletCounter,
                UiElementFactory.Hex("#6B7280"),
                TextAlignmentOptions.Bottom,
                new Vector2(90f, 30f),
                new Vector2(0f, -size.y * 0.5f + 15f),
                FontStyles.Bold);

            return new CargoPalletView(
                destination,
                surface,
                surface.GetComponent<Image>(),
                counter,
                color);
        }

        private static RectTransform CreateSafeArea(Transform parent)
        {
            var gameObject = new GameObject("Safe Area", typeof(RectTransform));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            if (Screen.width > 0 && Screen.height > 0)
            {
                var safeArea = Screen.safeArea;
                rect.anchorMin = new Vector2(
                    safeArea.xMin / Screen.width,
                    safeArea.yMin / Screen.height);
                rect.anchorMax = new Vector2(
                    safeArea.xMax / Screen.width,
                    safeArea.yMax / Screen.height);
            }
            else
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
            }

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private static CargoWorkStationView CreateWorkStation(
            Transform parent,
            WorkStationType type,
            string title,
            string recommendation,
            Vector2 position,
            Color color)
        {
            var backgroundColor = color;
            backgroundColor.a = 0.3f;
            var surface = UiElementFactory.Panel(
                parent,
                type.ToString(),
                backgroundColor,
                new Vector2(230f, 285f),
                position);

            UiElementFactory.Label(
                surface,
                "Station Title",
                title,
                24f,
                UiElementFactory.Hex("#172238"),
                TextAlignmentOptions.Center,
                new Vector2(215f, 38f),
                new Vector2(0f, 118f),
                FontStyles.Bold).raycastTarget = false;
            UiElementFactory.Label(
                surface,
                "Recommendation",
                recommendation,
                20f,
                UiElementFactory.Hex("#4B5563"),
                TextAlignmentOptions.Center,
                new Vector2(215f, 42f),
                new Vector2(0f, 84f)).raycastTarget = false;

            var employeeSlot = CreateAnchor(surface, "Employee Slot", new Vector2(0f, 24f));
            var cargoSlot = CreateAnchor(surface, "Cargo Slot", new Vector2(0f, -64f));
            var resultLabel = UiElementFactory.Label(
                surface,
                "Result",
                string.Empty,
                20f,
                UiElementFactory.Hex("#374151"),
                TextAlignmentOptions.Center,
                new Vector2(215f, 34f),
                new Vector2(0f, -118f),
                FontStyles.Bold);
            resultLabel.raycastTarget = false;

            return new CargoWorkStationView(
                type,
                surface,
                surface.GetComponent<Image>(),
                employeeSlot,
                cargoSlot,
                resultLabel,
                color);
        }

        private static RectTransform CreateAnchor(
            Transform parent,
            string name,
            Vector2 position)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            return rect;
        }

        private static void EnsureEventSystem(Transform owner)
        {
            if (EventSystem.current != null ||
                owner.GetComponentInChildren<EventSystem>(true) != null)
            {
                return;
            }

            var eventSystem = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule));
            eventSystem.transform.SetParent(owner, false);
        }
    }
}
