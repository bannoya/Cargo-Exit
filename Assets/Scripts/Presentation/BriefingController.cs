using BannoyasGames.CargoExit.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class BriefingController : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private Button startDayButton;

        [Header("Provisional day data")]
        [SerializeField] private string dayId = "test-day";
        [SerializeField] private int totalCargoCount = 8;
        [SerializeField] private int heavyCargoCount = 4;
        [SerializeField] private int fragileCargoCount = 4;
        [SerializeField] private string[] destinationIds = { "A", "B" };
        [SerializeField] private string objectiveId = "process-organize-dispatch";

        [Header("Variable texts")]
        [SerializeField] private TextMeshProUGUI totalCargoText;
        [SerializeField] private TextMeshProUGUI heavyCargoText;
        [SerializeField] private TextMeshProUGUI fragileCargoText;
        [SerializeField] private TextMeshProUGUI destinationsText;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Text formats")]
        [SerializeField] private string totalCargoFormat = "Total: {0} cajas";
        [SerializeField] private string heavyCargoFormat = "Pesadas: {0}";
        [SerializeField] private string fragileCargoFormat = "Frágiles: {0}";
        [SerializeField] private string destinationsFormat = "Destinos: {0}";

        private void Awake()
        {
            startDayButton.onClick.AddListener(StartDay);
        }

        private void Start()
        {
            PrepareAndShowBriefing();
        }

        private void OnDestroy()
        {
            startDayButton.onClick.RemoveListener(StartDay);
        }

        private void StartDay()
        {
            if (!PrepareAndShowBriefing())
            {
                return;
            }

            CargoSessionFlow.Instance.GoToEmployeeAssignment();
        }

        private bool PrepareAndShowBriefing()
        {
            CargoSessionFlow sessionFlow = CargoSessionFlow.Instance;

            if (sessionFlow == null)
            {
                startDayButton.interactable = false;
                messageText.text = "No hay una sesión activa.";
                return false;
            }

            CargoDayState state = sessionFlow.State;
            state.PrepareBriefing(
                dayId,
                totalCargoCount,
                heavyCargoCount,
                fragileCargoCount,
                destinationIds,
                objectiveId);

            totalCargoText.text = string.Format(totalCargoFormat, state.TotalCargoCount);
            heavyCargoText.text = string.Format(heavyCargoFormat, state.HeavyCargoCount);
            fragileCargoText.text = string.Format(fragileCargoFormat, state.FragileCargoCount);
            destinationsText.text = string.Format(
                destinationsFormat,
                string.Join(" y ", state.DestinationIds));
            messageText.text = string.Empty;
            startDayButton.interactable = true;
            return true;
        }
    }
}
