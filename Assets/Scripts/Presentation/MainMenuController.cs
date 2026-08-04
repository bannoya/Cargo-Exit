using BannoyasGames.CargoExit.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsButton;

        private void Awake()
        {
            newGameButton.onClick.AddListener(StartNewSession);
            continueButton.interactable = false;
            optionsButton.interactable = true;
        }

        private static void StartNewSession()
        {
            CargoSessionFlow.Instance.StartNewSession();
        }
    }
}
