using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShooterUI : MonoBehaviour
{
    [SerializeField] private BossHealthBar _bossHealthbar;
    [SerializeField] private PlayerHealthBar _playerHealthbar;
    [SerializeField] private Button _shootButton;
    [SerializeField] private TMP_Text _nextWaveText;
    [SerializeField] private ShooterWavesController _shooterController;

    private void OnEnable()
    {
        _shooterController.WavesStarted += OnWavesStarted;
    }

    private void OnWavesStarted()
    {
        EnablePlayerHealthBar();
        EnableShootButton();

        _shooterController.CurrentWaveFinished += OnCurrentWaveFinished;
        _shooterController.BossSpawned += OnBossSpawned;
        _shooterController.WavesFinished += OnWavesFinished;
    }

    private void OnCurrentWaveFinished(float duration)
    {
        StartCoroutine(ShowNextWaveMessage(duration));
    }

    private void OnBossSpawned(Monkey monkey)
    {
        EnableBossHealthBar(monkey);
        
        _shooterController.BossDefeated += OnBossDefeated;
    }

    private void OnBossDefeated()
    {
        DisableBossHealthBar();
    }

    private void OnWavesFinished()
    {
        DisablePlayerHealthBar();
        DisableShootButton();

        _shooterController.WavesStarted -= OnWavesStarted;
        _shooterController.BossSpawned -= OnBossSpawned;
        _shooterController.BossDefeated -= OnBossDefeated;
        _shooterController.WavesFinished -= OnWavesFinished;
    }

    private void EnablePlayerHealthBar()
    {
        _playerHealthbar.gameObject.SetActive(true);
    }

    private void EnableBossHealthBar(Monkey boss)
    {
        _bossHealthbar.gameObject.SetActive(true);
        _bossHealthbar.BindToBoss(boss);
    }

    private void EnableShootButton()
    {
        _shootButton.gameObject.SetActive(true);
    }

    private void EnableNextWaveText()
    {
        _nextWaveText.gameObject.SetActive(true);
    }

    private void DisablePlayerHealthBar()
    {
        _playerHealthbar.gameObject.SetActive(false);
    }

    private void DisableBossHealthBar()
    {
        _bossHealthbar.gameObject.SetActive(false);
    }

    private void DisableShootButton()
    {
        _shootButton.gameObject.SetActive(false);
    }

    private void DisableNextWaveText()
    {
        _nextWaveText.gameObject.SetActive(false);
    }

    private IEnumerator ShowNextWaveMessage(float duration)
    {
        EnableNextWaveText();

        yield return new WaitForSeconds(duration);

        DisableNextWaveText();
    }
}
