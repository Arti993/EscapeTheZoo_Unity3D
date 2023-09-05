using UnityEngine;

public class LevelsChanger : MonoBehaviour
{
    [SerializeField] private Level[] _levels;
    [SerializeField] private GameObject _victoryLocation;
    [SerializeField] private GameObject _gameFinishScreen;

    private Level _currentLevel;
    private Level _previousLevel;
    private int _currentLevelIndex = 0;
    private int _differenceValueBetweenLevels = 2;

    public Level CurrentLevel => _currentLevel;
    public Level PreviousLevel => _previousLevel;

    public static LevelsChanger Instance;

    private void Awake()
    {
        Instance = this;
        _currentLevel = _levels[_currentLevelIndex];
        _currentLevel.gameObject.SetActive(true);

        _currentLevel.StartPlayMusic();

        _currentLevel.AllActionsFinished += OnNextLevelStarted;
        _currentLevel.Passed += OnLevelPassed;
    }

    private void OnLevelPassed()
    {
        ChangeLevel();
    }

    private void OnNextLevelStarted()
    {
        int levelToDisableIndex = _currentLevelIndex - _differenceValueBetweenLevels;

        if(levelToDisableIndex >= 0)
        {
            Level levelToDisable = _levels[levelToDisableIndex];
            levelToDisable.gameObject.SetActive(false);
        }

        _previousLevel.AllActionsFinished -= OnNextLevelStarted;
        _previousLevel.Passed -= OnLevelPassed;
    }

    private void ChangeLevel()
    {
        if(_currentLevelIndex < _levels.Length - 1)
        {
            _currentLevel.StopPlayMusic();

            _previousLevel = _currentLevel;
            _currentLevelIndex++;
            _currentLevel = _levels[_currentLevelIndex];
            _currentLevel.gameObject.SetActive(true);

            _currentLevel.StartPlayMusic();  

            _currentLevel.AllActionsFinished += OnNextLevelStarted;
            _currentLevel.Passed += OnLevelPassed;
        }
        else
        {
            _currentLevel.StopPlayMusic();
            _victoryLocation.SetActive(true);
            _gameFinishScreen.SetActive(true);
        }
    }
}
