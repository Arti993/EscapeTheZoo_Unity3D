using System;
using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class PlayerThrowAttackController : MonoBehaviour
{
    [SerializeField] private ObjectPool _playerProjectilesPool;
    [SerializeField] private ShooterController _shooterController;
    [SerializeField] private AudioSource _throwSound;

    private bool _isAttacking = false;
    private float _attackDelayTime = 0.5f;

    public event UnityAction Attacked;

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

    private void OnEnable()
    {
        _shooterController.ShooterStarted += OnShooterStarted;
    }

    private void OnShooterStarted()
    {
        _playerProjectilesPool.gameObject.SetActive(true);

        _shooterController.ShooterFinished += OnShooterFinished;
    }

    private void OnShooterFinished()
    {
        if (_playerProjectilesPool != null)
            _playerProjectilesPool.gameObject.SetActive(false);

        _shooterController.ShooterStarted -= OnShooterStarted;
        _shooterController.ShooterFinished -= OnShooterFinished;

        ThirdPersonController.Instance.DisableShooting();
    }

    private IEnumerator WaitAttackDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        _isAttacking = false;
    }
}
