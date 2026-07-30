using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;
using TMPro;
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
