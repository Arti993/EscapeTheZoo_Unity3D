using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _mainScreen;
    [SerializeField] private GameObject _authorsScreen;
    [SerializeField] private GameObject _controlsScreen;

    private int _gameSceneNumber = 1;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(_gameSceneNumber);

        Destroy(MenuMusicController.Instance.gameObject);
    }

    public void OpenAuthorsScreen()
    {
        _mainScreen.SetActive(false);
        _authorsScreen.SetActive(true);
    }

    public void OpenControlsScreen()
    {
        _mainScreen.SetActive(false);
        _controlsScreen.SetActive(true);
    }

    public void CloseAuthorsScreen()
    {
        _authorsScreen.SetActive(false);
        _mainScreen.SetActive(true);
        
    }

    public void CloseControlsScreen()
    {
        _controlsScreen.SetActive(false);
        _mainScreen.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
