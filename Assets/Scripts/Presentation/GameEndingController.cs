using BannoyasGames.CargoExit.Application;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class GameEndingController : MonoBehaviour
    {
        [SerializeField] private Button returnButton;

        private void Awake()
        {
            returnButton.onClick.AddListener(ReturnToMenu);
        }

        private static void ReturnToMenu()
        {
            CargoSessionFlow.Instance.ReturnToMainMenu();
        }
    }
}
