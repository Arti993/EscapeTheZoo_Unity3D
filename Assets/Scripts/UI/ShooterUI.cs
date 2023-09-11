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
    [SerializeField] private MonkeyWavesSpawner _monkeyWavesSpawner;

    private void OnEnable()
    {
        _monkeyWavesSpawner.WavesStarted += OnWavesStarted;
    }

    private void OnWavesStarted()
    {
        EnablePlayerHealthBar();
        EnableShootButton();

        _monkeyWavesSpawner.CurrentWaveFinished += OnCurrentWaveFinished;
        _monkeyWavesSpawner.BossSpawned += OnBossSpawned;
        _monkeyWavesSpawner.WavesFinished += OnWavesFinished;
    }

    private void OnCurrentWaveFinished(float duration)
    {
        StartCoroutine(ShowNextWaveMessage(duration));
    }

    private void OnBossSpawned(Monkey monkey)
    {
        EnableBossHealthBar(monkey);
        
        _monkeyWavesSpawner.BossDefeated += OnBossDefeated;
    }

    private void OnBossDefeated()
    {
        DisableBossHealthBar();
    }

    private void OnWavesFinished()
    {
        DisablePlayerHealthBar();
        DisableShootButton();

        _monkeyWavesSpawner.WavesStarted -= OnWavesStarted;
        _monkeyWavesSpawner.BossSpawned -= OnBossSpawned;
        _monkeyWavesSpawner.BossDefeated -= OnBossDefeated;
        _monkeyWavesSpawner.WavesFinished -= OnWavesFinished;
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
