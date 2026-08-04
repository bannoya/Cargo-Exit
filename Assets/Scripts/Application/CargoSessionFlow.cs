using UnityEngine;
using UnityEngine.SceneManagement;

namespace BannoyasGames.CargoExit.Application
{
    public sealed class CargoSessionFlow : MonoBehaviour
    {
        private const string MainMenuScene = "MainMenu";
        private const string TutorialScene = "Tutorial";
        private const string BriefingScene = "Briefing";
        private const string EmployeeAssignmentScene = "EmployeeAssignment";
        private const string CargoProcessingScene = "CargoProcessing";
        private const string PalletAssemblyScene = "PalletAssembly";
        private const string TruckAssignmentScene = "TruckAssignment";
        private const string DaySummaryScene = "DaySummary";
        private const string IntermissionScene = "Intermission";
        private const string GameEndingScene = "GameEnding";

        public static CargoSessionFlow Instance { get; private set; }

        public CargoDayState State { get; } = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void StartNewSession()
        {
            State.Reset();
            GoToTutorial();
        }

        public void StartNextSession()
        {
            State.Reset();
            GoToBriefing();
        }

        public void GoToTutorial() => Load(TutorialScene);

        public void GoToBriefing() => Load(BriefingScene);

        public void GoToEmployeeAssignment() => Load(EmployeeAssignmentScene);

        public void GoToCargoProcessing() => Load(CargoProcessingScene);

        public void GoToPalletAssembly() => Load(PalletAssemblyScene);

        public void GoToTruckAssignment() => Load(TruckAssignmentScene);

        public void GoToDaySummary() => Load(DaySummaryScene);

        public void GoToIntermission() => Load(IntermissionScene);

        public void GoToGameEnding() => Load(GameEndingScene);

        public void ReturnToMainMenu() => Load(MainMenuScene);

        private static void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
