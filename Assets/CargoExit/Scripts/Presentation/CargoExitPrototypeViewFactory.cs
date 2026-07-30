using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;
using UnityEngine;
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
            var canvasRect = root.GetComponent<RectTransform>();

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var background = UiElementFactory.Panel(
                root.transform,
                "Background",
                UiElementFactory.Hex("#FFF4D6"),
                new Vector2(1080f, 1920f),
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
                TextAnchor.MiddleCenter,
                new Vector2(700f, 48f),
                new Vector2(0f, 887f),
                FontStyle.Bold);

            UiElementFactory.Label(
                root.transform,
                "Title",
                "ORDENÁ EL DEPÓSITO",
                CargoExitTypography.Title,
                UiElementFactory.Hex("#172238"),
                TextAnchor.MiddleCenter,
                new Vector2(930f, 82f),
                new Vector2(0f, 824f),
                FontStyle.Bold);

            var statusLabel = UiElementFactory.Label(
                root.transform,
                "Status",
                string.Empty,
                CargoExitTypography.Status,
                UiElementFactory.Hex("#374151"),
                TextAnchor.MiddleCenter,
                new Vector2(860f, 66f),
                new Vector2(-40f, 762f),
                FontStyle.Bold);

            var comboLabel = UiElementFactory.Label(
                root.transform,
                "Combo",
                string.Empty,
                CargoExitTypography.Combo,
                UiElementFactory.Hex("#D97706"),
                TextAnchor.MiddleRight,
                new Vector2(260f, 60f),
                new Vector2(365f, 762f),
                FontStyle.Bold);

            var pallets = new Dictionary<CargoDestination, CargoPalletView>
            {
                [CargoDestination.A] = CreatePallet(
                    canvas.transform,
                    CargoDestination.A,
                    new Vector2(-320f, 620f),
                    new Vector2(280f, 190f)),
                [CargoDestination.B] = CreatePallet(
                    canvas.transform,
                    CargoDestination.B,
                    new Vector2(0f, 620f),
                    new Vector2(280f, 190f)),
                [CargoDestination.C] = CreatePallet(
                    canvas.transform,
                    CargoDestination.C,
                    new Vector2(320f, 620f),
                    new Vector2(280f, 190f)),
                [CargoDestination.D] = CreatePallet(
                    canvas.transform,
                    CargoDestination.D,
                    new Vector2(-400f, 250f),
                    new Vector2(250f, 310f)),
                [CargoDestination.E] = CreatePallet(
                    canvas.transform,
                    CargoDestination.E,
                    new Vector2(-400f, -145f),
                    new Vector2(250f, 310f)),
                [CargoDestination.F] = CreatePallet(
                    canvas.transform,
                    CargoDestination.F,
                    new Vector2(400f, 250f),
                    new Vector2(250f, 310f)),
                [CargoDestination.G] = CreatePallet(
                    canvas.transform,
                    CargoDestination.G,
                    new Vector2(400f, -145f),
                    new Vector2(250f, 310f))
            };

            var pileSurface = UiElementFactory.Panel(
                root.transform,
                "Unsorted Boxes",
                new Color(1f, 1f, 1f, 0.48f),
                new Vector2(465f, 955f),
                new Vector2(0f, 70f));

            UiElementFactory.Label(
                pileSurface,
                "PileTitle",
                "CAJAS DESORDENADAS",
                CargoExitTypography.PileTitle,
                UiElementFactory.Hex("#6B7280"),
                TextAnchor.UpperCenter,
                new Vector2(430f, 60f),
                new Vector2(0f, 432f),
                FontStyle.Bold);

            UiElementFactory.Label(
                root.transform,
                "Hint",
                "Arrastrá cada caja al pallet con la misma letra",
                CargoExitTypography.Hint,
                UiElementFactory.Hex("#6B7280"),
                TextAnchor.MiddleCenter,
                new Vector2(900f, 70f),
                new Vector2(0f, -790f));

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
            var statusLabel = interfaceTransform?.Find("Status")?.GetComponent<Text>();
            var comboLabel = interfaceTransform?.Find("Combo")?.GetComponent<Text>();
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
                var counter = surface?.Find("Counter")?.GetComponent<Text>();
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
                TextAnchor.MiddleCenter,
                new Vector2(150f, 100f),
                new Vector2(0f, 18f),
                FontStyle.Bold);

            var counter = UiElementFactory.Label(
                surface,
                "Counter",
                "0 / 2",
                CargoExitTypography.PalletCounter,
                UiElementFactory.Hex("#6B7280"),
                TextAnchor.LowerCenter,
                new Vector2(180f, 46f),
                new Vector2(0f, -size.y * 0.5f + 30f),
                FontStyle.Bold);

            return new CargoPalletView(
                destination,
                surface,
                surface.GetComponent<Image>(),
                counter,
                color);
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
