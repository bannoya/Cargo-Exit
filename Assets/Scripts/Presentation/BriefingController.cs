using BannoyasGames.CargoExit.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class BriefingController : MonoBehaviour
    {
        [SerializeField] private Button startDayButton;

        private void Awake()
        {
            startDayButton.onClick.AddListener(StartDay);
        }

        private static void StartDay()
        {
            CargoSessionFlow.Instance.GoToEmployeeAssignment();
        }
    }
}
