using BannoyasGames.CargoExit.Application;
using BannoyasGames.CargoExit.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class DaySummaryController : MonoBehaviour
    {
        [SerializeField] private Text normalCountLabel;
        [SerializeField] private Text inefficientCountLabel;
        [SerializeField] private Text damagedCountLabel;
        [SerializeField] private Text causeLabel;
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            continueButton.onClick.AddListener(Continue);
        }

        private void Start()
        {
            var state = CargoSessionFlow.Instance.State;
            if (!state.ProcessingCompleted)
            {
                normalCountLabel.text = "Normales: 0";
                inefficientCountLabel.text = "Ineficientes: 0";
                damagedCountLabel.text = "Dañadas: 0";
                causeLabel.text = "No hay resultados de procesamiento.";
                return;
            }

            var normal = Count(
                state.HeavyCargoResult,
                state.FragileCargoResult,
                CargoProcessingResult.Normal);
            var inefficient = Count(
                state.HeavyCargoResult,
                state.FragileCargoResult,
                CargoProcessingResult.Inefficient);
            var damaged = Count(
                state.HeavyCargoResult,
                state.FragileCargoResult,
                CargoProcessingResult.Damaged);

            normalCountLabel.text = $"Normales: {normal}";
            inefficientCountLabel.text = $"Ineficientes: {inefficient}";
            damagedCountLabel.text = $"Dañadas: {damaged}";
            causeLabel.text =
                $"Carga pesada: {ResultLabel(state.HeavyCargoResult)}\n" +
                $"Carga frágil: {ResultLabel(state.FragileCargoResult)}";
        }

        private static int Count(
            CargoProcessingResult first,
            CargoProcessingResult second,
            CargoProcessingResult expected)
        {
            var count = first == expected ? 1 : 0;
            return second == expected ? count + 1 : count;
        }

        private static string ResultLabel(CargoProcessingResult result)
        {
            return result switch
            {
                CargoProcessingResult.Inefficient => "INEFICIENTE",
                CargoProcessingResult.Damaged => "DAÑADA",
                _ => "NORMAL"
            };
        }

        private static void Continue()
        {
            CargoSessionFlow.Instance.GoToIntermission();
        }
    }
}
