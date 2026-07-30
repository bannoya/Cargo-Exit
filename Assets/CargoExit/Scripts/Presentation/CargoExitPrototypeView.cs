using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    internal sealed class CargoExitPrototypeView
    {
        public CargoExitPrototypeView(
            Canvas canvas,
            RectTransform canvasRect,
            RectTransform pileSurface,
            Text statusLabel,
            Text comboLabel,
            Dictionary<CargoDestination, CargoPalletView> pallets)
        {
            Canvas = canvas;
            CanvasRect = canvasRect;
            PileSurface = pileSurface;
            StatusLabel = statusLabel;
            ComboLabel = comboLabel;
            Pallets = pallets;
        }

        public Canvas Canvas { get; }

        public RectTransform CanvasRect { get; }

        public RectTransform PileSurface { get; }

        public Text StatusLabel { get; }

        public Text ComboLabel { get; }

        public IReadOnlyDictionary<CargoDestination, CargoPalletView> Pallets { get; }
    }

    internal sealed class CargoPalletView
    {
        public CargoPalletView(
            CargoDestination destination,
            RectTransform surface,
            Image background,
            Text counter,
            Color color)
        {
            Destination = destination;
            Surface = surface;
            Background = background;
            Counter = counter;
            Color = color;
        }

        public CargoDestination Destination { get; }

        public RectTransform Surface { get; }

        public Image Background { get; }

        public Text Counter { get; }

        public Color Color { get; }
    }
}
