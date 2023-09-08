using UnityEngine;

[RequireComponent(typeof(ShooterController))]
public class ShooterMusicController : MonoBehaviour
{
    [SerializeField] private AudioSource _bossTheme;

    private ShooterController _shooterController;

    private void OnEnable()
    {
        _shooterController = GetComponent<ShooterController>();
        _shooterController.BossSpawned += OnBossSpawned;
        _shooterController.ShooterRestarted += OnShooterRestarted;
    }

    private void OnBossSpawned(Monkey boss)
    {
        LevelsChanger.Instance.CurrentLevel.StopPlayMusic();

        _bossTheme.Play();

        _shooterController.BossDefeated += OnBossDefeated;
    }

    private void OnBossDefeated()
    {
        _bossTheme.Stop();

        LevelsChanger.Instance.CurrentLevel.StartPlayMusic();

        _shooterController.BossSpawned -= OnBossSpawned;
        _shooterController.BossDefeated -= OnBossDefeated;
        _shooterController.ShooterRestarted -= OnShooterRestarted;
    }

    private void OnShooterRestarted()
    {
        if (_bossTheme.isPlaying)
        {
            _bossTheme.Stop();

            LevelsChanger.Instance.CurrentLevel.StartPlayMusic();
        }
    }
}
