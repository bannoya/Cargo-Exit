using System.Collections.Generic;
using BannoyasGames.CargoExit.Core;
using TMPro;
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
            TMP_Text statusLabel,
            TMP_Text comboLabel,
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

        public TMP_Text StatusLabel { get; }

        public TMP_Text ComboLabel { get; }

        public IReadOnlyDictionary<CargoDestination, CargoPalletView> Pallets { get; }
    }

    internal sealed class CargoPalletView
    {
        public CargoPalletView(
            CargoDestination destination,
            RectTransform surface,
            Image background,
            TMP_Text counter,
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

        public TMP_Text Counter { get; }

        public Color Color { get; }
    }
}
