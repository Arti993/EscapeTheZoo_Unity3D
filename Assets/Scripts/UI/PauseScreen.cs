using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private GameObject _panel;
    [SerializeField] private Canvas _UIControls;

    private int _mainMenuSceneNumber = 0;

    private void OnEnable()
    {
        _continueButton.onClick.AddListener(OnContinueButtonClick);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);

    }

    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
        _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClick);
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;

        ThirdPersonController.Instance.DisableInput();

        _panel.SetActive(true);

        _UIControls.gameObject.SetActive(false);

        Time.timeScale = 0;
    }

    private void OnContinueButtonClick()
    {
        ClosePanel();
    }

    private void OnMainMenuButtonClick()
    {
        ClosePanel();

        SceneManager.LoadScene(_mainMenuSceneNumber);
    }

    private void ClosePanel()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;

        _panel.SetActive(false);

        _UIControls.gameObject.SetActive(true);

        ThirdPersonController.Instance.EnableInput();
    }
}