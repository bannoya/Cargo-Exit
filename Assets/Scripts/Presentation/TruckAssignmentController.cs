using BannoyasGames.CargoExit.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class TruckAssignmentController : MonoBehaviour
    {
        [SerializeField] private Button continueButton;

        private void Awake()
        {
            continueButton.onClick.AddListener(Continue);
        }

        private static void Continue()
        {
            CargoSessionFlow.Instance.GoToDaySummary();
        }
    }
}
