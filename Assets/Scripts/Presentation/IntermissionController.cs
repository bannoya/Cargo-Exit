using BannoyasGames.CargoExit.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class IntermissionController : MonoBehaviour
    {
        [SerializeField] private Button nextSessionButton;
        [SerializeField] private Button finishTestButton;

        private void Awake()
        {
            nextSessionButton.onClick.AddListener(StartNextSession);
            finishTestButton.onClick.AddListener(FinishTest);
        }

        private static void StartNextSession()
        {
            CargoSessionFlow.Instance.StartNextSession();
        }

        private static void FinishTest()
        {
            CargoSessionFlow.Instance.GoToGameEnding();
        }
    }
}
