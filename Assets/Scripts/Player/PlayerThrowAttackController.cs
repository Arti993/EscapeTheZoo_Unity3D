using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;

public class PlayerThrowAttackController : MonoBehaviour
{
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

    private IEnumerator WaitAttackDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        _isAttacking = false;
    }
}
