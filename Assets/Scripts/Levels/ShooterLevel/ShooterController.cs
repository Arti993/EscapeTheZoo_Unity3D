using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Player _player;
    [SerializeField] private ObjectPool _playerProjectilesPool;
    [SerializeField] private Key _keyTemplate;
    [SerializeField] private BossHealthBar _bossHealthbar;
    [SerializeField] private PlayerHealthBar _playerHealthbar;
    [SerializeField] private Button _shootButton;
    [SerializeField] private TMP_Text _nextWaveText;
    [SerializeField] private AudioSource _bossTheme;

    private List<Monkey> _aliveMonkeys;
    private Wave _currentWave;
    private int _currentWaveNumber = 0;
    private float _pauseTimeBetweenWaves = 4;

    public event Action<int, int> EnemyCountChanged;

    public void StartWork()
    {
        _playerHealthbar.gameObject.SetActive(true);

        _shootButton.gameObject.SetActive(true);

        _playerProjectilesPool.gameObject.SetActive(true);

        _aliveMonkeys = new List<Monkey>();

        SetWave(_currentWaveNumber);

        EnemyCountChanged?.Invoke(0, 1);
    }

    public void NextWave()
    {
        SetWave(++_currentWaveNumber);
    }

    private void OnEnable()
    {
        _player.Dying += OnPlayerDying;
    }

    private void OnDisable()
    {
        _player.Dying -= OnPlayerDying;

        if (_playerProjectilesPool != null)
            _playerProjectilesPool.gameObject.SetActive(false);
    }

    private void InstantiateMonkey(int index)
    {
        Monkey monkey = Instantiate(_currentWave.Template, _spawnPoints[index].position, _spawnPoints[index].rotation,
            _spawnPoints[index]).GetComponent<Monkey>();

        _aliveMonkeys.Add(monkey);

        monkey.Dying += OnMonkeyDying;

        if (monkey.gameObject.TryGetComponent(out Boss boss))
        {
            _bossHealthbar.gameObject.SetActive(true);

            _bossHealthbar.BindToBoss(monkey);

            LevelsChanger.Instance.CurrentLevel.StopPlayMusic();

            _bossTheme.Play();
        }
    }

    private void SpawnCurrentWave()
    {
        int spawnPointIndex;

        for (int i = 0; i < _currentWave.MonkeysCount; i++)
        {
            spawnPointIndex = i;
            InstantiateMonkey(spawnPointIndex);

            EnemyCountChanged?.Invoke(_aliveMonkeys.Count, _currentWave.MonkeysCount);
        }
    }

    private void SetWave(int index)
    {
        _currentWave = _waves[index];
        SpawnCurrentWave();
    }

    private void OnMonkeyDying(Monkey monkey)
    {
        _aliveMonkeys.Remove(monkey);
        monkey.Dying -= OnMonkeyDying;

        if (_aliveMonkeys.Count == 0)
        {
            if (_currentWaveNumber < _waves.Count - 1)
            {
                StartCoroutine(WaitBeforeNextWave(_pauseTimeBetweenWaves));
            }
            else
            {
                if (monkey.gameObject.TryGetComponent(out Boss boss))
                {
                    _bossHealthbar.gameObject.SetActive(false);

                    _bossTheme.Stop();

                    LevelsChanger.Instance.CurrentLevel.StartPlayMusic();
                }

                ThirdPersonController.Instance.DisableShooting();

                _playerHealthbar.gameObject.SetActive(false);

                _shootButton.gameObject.SetActive(false);

                Instantiate(_keyTemplate, monkey.transform.position, Quaternion.identity,
                    LevelsChanger.Instance.CurrentLevel.transform);

                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnPlayerDying()
    {
        StartCoroutine(WaitBeforeRestart(_pauseTimeBetweenWaves));
    }

    private IEnumerator WaitBeforeNextWave(float duration)
    {
        _nextWaveText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        _nextWaveText.gameObject.SetActive(false);

        NextWave();
    }

    private IEnumerator WaitBeforeRestart(float duration)
    {
        yield return new WaitForSeconds(duration);

        foreach (var monkey in _aliveMonkeys)
            Destroy(monkey.gameObject);

        _aliveMonkeys.Clear();

        _currentWaveNumber = 0;

        if (_bossTheme.isPlaying)
        {
            _bossTheme.Stop();

            LevelsChanger.Instance.CurrentLevel.StartPlayMusic();
        }

        SetWave(_currentWaveNumber);
    }
}


[System.Serializable]
public class Wave
{
    public GameObject Template;
    public int MonkeysCount;
}
