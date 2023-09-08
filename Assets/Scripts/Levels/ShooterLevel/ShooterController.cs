using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;


public class ShooterController : MonoBehaviour
{
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Key _keyTemplate;
    [SerializeField] private ShooterUI _shooterUI;

    private List<Monkey> _aliveMonkeys = new List<Monkey>();
    private Wave _currentWave;
    private int _currentWaveNumber = 0;
    private float _pauseTimeBetweenWaves = 4;

    public event Action ShooterStarted;
    public event Action<float> WaveEnded;
    public event Action<Monkey> BossSpawned;
    public event Action BossDefeated;
    public event Action ShooterRestarted;
    public event Action ShooterFinished;

    private void OnEnable()
    {
        SetWave(_currentWaveNumber);

        Player.Instance.Dying += OnPlayerDying;
    }

    private void Start()
    {
        ShooterStarted?.Invoke();
    }

    private void OnDisable()
    {
        Player.Instance.Dying -= OnPlayerDying;
    }

    private void NextWave()
    {
        SetWave(++_currentWaveNumber);
    }

    private void SetWave(int index)
    {
        _currentWave = _waves[index];
        SpawnCurrentWave();
    }

    private void SpawnCurrentWave()
    {
        int spawnPointIndex;

        for (int i = 0; i < _currentWave.MonkeysCount; i++)
        {
            spawnPointIndex = i;
            InstantiateMonkey(spawnPointIndex);
        }
    }

    private void InstantiateMonkey(int index)
    {
        Monkey monkey = Instantiate(_currentWave.Template, _spawnPoints[index].position, _spawnPoints[index].rotation,
            _spawnPoints[index]).GetComponent<Monkey>();

        _aliveMonkeys.Add(monkey);

        monkey.Dying += OnMonkeyDying;

        if (monkey.gameObject.TryGetComponent(out Boss boss))
            BossSpawned?.Invoke(monkey);
    }

    private void OnMonkeyDying(Monkey monkey)
    {
        _aliveMonkeys.Remove(monkey);
        monkey.Dying -= OnMonkeyDying;

        if (_aliveMonkeys.Count == 0)
        {
            if (_currentWaveNumber < _waves.Count - 1)
            {
                StartCoroutine(WaitBeforeNextWave());

                WaveEnded?.Invoke(_pauseTimeBetweenWaves);
            }
            else
            {
                if (monkey.gameObject.TryGetComponent(out Boss boss))
                    BossDefeated?.Invoke();

                Instantiate(_keyTemplate, monkey.transform.position, Quaternion.identity,
                    LevelsChanger.Instance.CurrentLevel.transform);

                ShooterFinished?.Invoke();

                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnPlayerDying()
    {
        StartCoroutine(WaitBeforeRestart());
    }

    private IEnumerator WaitBeforeNextWave()
    {
        yield return new WaitForSeconds(_pauseTimeBetweenWaves);

        NextWave();
    }

    private IEnumerator WaitBeforeRestart()
    {
        yield return new WaitForSeconds(_pauseTimeBetweenWaves);

        ShooterRestarted?.Invoke();

        foreach (var monkey in _aliveMonkeys)
            Destroy(monkey.gameObject);

        _aliveMonkeys.Clear();

        _currentWaveNumber = 0;

        SetWave(_currentWaveNumber);
    }
}

[System.Serializable]
public class Wave
{
    public GameObject Template;
    public int MonkeysCount;
}