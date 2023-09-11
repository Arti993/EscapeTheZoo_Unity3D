using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class PlayerThrowAttackController : MonoBehaviour
{
    [SerializeField] private ObjectPool _playerProjectilesPool;
    [SerializeField] private MonkeyWavesSpawner _monkeyWavesSpawner;
    [SerializeField] private AudioSource _throwSound;

    private bool _isAttacking = false;
    private float _attackDelayTime = 0.5f;

    public event UnityAction Attacked;

    private void OnEnable()
    {
        _monkeyWavesSpawner.WavesStarted += OnWavesStarted;
    }

    public void OnThrowAttack()
    {
        if(ThirdPersonController.Instance.IsShootingEnable)
        {
            if (_isAttacking == false)
            {
                _isAttacking = true;
                Attacked?.Invoke();
                _throwSound.Play();
                StartCoroutine(WaitAttackDelay(_attackDelayTime));
            }
        }
    }

    private void OnWavesStarted()
    {
        _playerProjectilesPool.gameObject.SetActive(true);

        _monkeyWavesSpawner.WavesFinished += OnWavesFinished;
    }

    private void OnWavesFinished()
    {
        if (_playerProjectilesPool != null)
            _playerProjectilesPool.gameObject.SetActive(false);

        _monkeyWavesSpawner.WavesStarted -= OnWavesStarted;
        _monkeyWavesSpawner.WavesFinished -= OnWavesFinished;

        ThirdPersonController.Instance.DisableShooting();
    }

    private IEnumerator WaitAttackDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        _isAttacking = false;
    }
}
