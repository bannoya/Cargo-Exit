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

    internal sealed class CargoExitWorkTestView
    {
        public CargoExitWorkTestView(
            RectTransform root,
            TMP_Text titleLabel,
            TMP_Text statusLabel,
            TMP_Text hintLabel,
            RectTransform employeePool,
            Button confirmButton,
            RectTransform warningPanel,
            TMP_Text warningLabel,
            RectTransform processingPile,
            RectTransform finalPanel,
            TMP_Text finalLabel,
            Dictionary<WorkStationType, CargoWorkStationView> stations)
        {
            Root = root;
            TitleLabel = titleLabel;
            StatusLabel = statusLabel;
            HintLabel = hintLabel;
            EmployeePool = employeePool;
            ConfirmButton = confirmButton;
            WarningPanel = warningPanel;
            WarningLabel = warningLabel;
            ProcessingPile = processingPile;
            FinalPanel = finalPanel;
            FinalLabel = finalLabel;
            Stations = stations;
        }

        public RectTransform Root { get; }

        public TMP_Text TitleLabel { get; }

        public TMP_Text StatusLabel { get; }

        public TMP_Text HintLabel { get; }

        public RectTransform EmployeePool { get; }

        public Button ConfirmButton { get; }

        public RectTransform WarningPanel { get; }

        public TMP_Text WarningLabel { get; }

        public RectTransform ProcessingPile { get; }

        public RectTransform FinalPanel { get; }

        public TMP_Text FinalLabel { get; }

        public IReadOnlyDictionary<WorkStationType, CargoWorkStationView> Stations { get; }
    }

    internal sealed class CargoWorkStationView
    {
        public CargoWorkStationView(
            WorkStationType type,
            RectTransform surface,
            Image background,
            RectTransform employeeSlot,
            RectTransform cargoSlot,
            TMP_Text resultLabel,
            Color color)
        {
            Type = type;
            Surface = surface;
            Background = background;
            EmployeeSlot = employeeSlot;
            CargoSlot = cargoSlot;
            ResultLabel = resultLabel;
            Color = color;
        }

        public WorkStationType Type { get; }

        public RectTransform Surface { get; }

        public Image Background { get; }

        public RectTransform EmployeeSlot { get; }

        public RectTransform CargoSlot { get; }

        public TMP_Text ResultLabel { get; }

        public Color Color { get; }
    }
}
