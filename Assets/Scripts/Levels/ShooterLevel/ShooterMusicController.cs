using UnityEngine;

[RequireComponent(typeof(MonkeyWavesSpawner))]
public class ShooterMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource _bossTheme;

    private MonkeyWavesSpawner _monkeyWavesSpawner;

    private void OnEnable()
    {
        _monkeyWavesSpawner = GetComponent<MonkeyWavesSpawner>();
        _monkeyWavesSpawner.BossSpawned += OnBossSpawned;
        _monkeyWavesSpawner.WavesRestarted += OnWavesRestarted;
    }

    private void OnBossSpawned(Monkey boss)
    {
        LevelsChanger.Instance.CurrentLevel.StopPlayMusic();

        _bossTheme.Play();

        _monkeyWavesSpawner.BossDefeated += OnBossDefeated;
    }

    private void OnBossDefeated()
    {
        _bossTheme.Stop();

        LevelsChanger.Instance.CurrentLevel.StartPlayMusic();

        _monkeyWavesSpawner.BossSpawned -= OnBossSpawned;
        _monkeyWavesSpawner.BossDefeated -= OnBossDefeated;
        _monkeyWavesSpawner.WavesRestarted -= OnWavesRestarted;
    }

    private void OnWavesRestarted()
    {
        if (_bossTheme.isPlaying)
        {
            _bossTheme.Stop();

            LevelsChanger.Instance.CurrentLevel.StartPlayMusic();
        }
    }
}
