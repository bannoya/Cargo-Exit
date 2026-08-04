using BannoyasGames.CargoExit.Application;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BannoyasGames.CargoExit.Presentation
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject mainButtons;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Button optionsBackButton;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private Button creditsBackButton;
        [SerializeField] private GameObject confirmationPanel;
        [SerializeField] private Button confirmationConfirmButton;
        [SerializeField] private Button confirmationCancelButton;
        [SerializeField] private TMP_Text continueAvailabilityText;
        [SerializeField] private TMP_Text versionText;

        private bool vibrationEnabled;

        public bool VibrationEnabled => vibrationEnabled;

        private void Awake()
        {
            newGameButton.onClick.AddListener(StartNewSession);
            optionsButton.onClick.AddListener(OpenOptions);
            creditsButton.onClick.AddListener(OpenCredits);
            optionsBackButton.onClick.AddListener(ClosePanels);
            creditsBackButton.onClick.AddListener(ClosePanels);
            confirmationConfirmButton.onClick.AddListener(ClosePanels);
            confirmationCancelButton.onClick.AddListener(ClosePanels);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            vibrationToggle.onValueChanged.AddListener(SetVibration);

            masterVolumeSlider.SetValueWithoutNotify(AudioListener.volume);
            vibrationEnabled = vibrationToggle.isOn;
            versionText.text = $"VERSIÓN {UnityEngine.Application.version}";
            SetContinueAvailable(false);
            ClosePanels();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseActivePanel();
            }
        }

        private static void StartNewSession()
        {
            CargoSessionFlow.Instance.StartNewSession();
        }

        public void SetContinueAvailable(bool available)
        {
            continueButton.interactable = available;
            continueAvailabilityText.text = available
                ? "DISPONIBLE"
                : "NO DISPONIBLE";
        }

        public void OpenConfirmation()
        {
            OpenPanel(confirmationPanel);
        }

        private void OpenOptions()
        {
            OpenPanel(optionsPanel);
        }

        private void OpenCredits()
        {
            OpenPanel(creditsPanel);
        }

        private void OpenPanel(GameObject panel)
        {
            mainButtons.SetActive(false);
            optionsPanel.SetActive(panel == optionsPanel);
            creditsPanel.SetActive(panel == creditsPanel);
            confirmationPanel.SetActive(panel == confirmationPanel);
        }

        private void CloseActivePanel()
        {
            if (confirmationPanel.activeSelf ||
                creditsPanel.activeSelf ||
                optionsPanel.activeSelf)
            {
                ClosePanels();
            }
        }

        private void ClosePanels()
        {
            optionsPanel.SetActive(false);
            creditsPanel.SetActive(false);
            confirmationPanel.SetActive(false);
            mainButtons.SetActive(true);
        }

        private static void SetMasterVolume(float volume)
        {
            AudioListener.volume = volume;
        }

        private void SetVibration(bool enabled)
        {
            vibrationEnabled = enabled;
        }
    }
}
