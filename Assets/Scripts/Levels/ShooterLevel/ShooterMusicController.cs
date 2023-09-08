using UnityEngine;

[RequireComponent(typeof(ShooterWavesController))]
public class ShooterMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource _bossTheme;

    private ShooterWavesController _shooterWavesController;

    private void OnEnable()
    {
        _shooterWavesController = GetComponent<ShooterWavesController>();
        _shooterWavesController.BossSpawned += OnBossSpawned;
        _shooterWavesController.WavesRestarted += OnWavesRestarted;
    }

    private void OnBossSpawned(Monkey boss)
    {
        LevelsChanger.Instance.CurrentLevel.StopPlayMusic();

        _bossTheme.Play();

        _shooterWavesController.BossDefeated += OnBossDefeated;
    }

    private void OnBossDefeated()
    {
        _bossTheme.Stop();

        LevelsChanger.Instance.CurrentLevel.StartPlayMusic();

        _shooterWavesController.BossSpawned -= OnBossSpawned;
        _shooterWavesController.BossDefeated -= OnBossDefeated;
        _shooterWavesController.WavesRestarted -= OnWavesRestarted;
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
