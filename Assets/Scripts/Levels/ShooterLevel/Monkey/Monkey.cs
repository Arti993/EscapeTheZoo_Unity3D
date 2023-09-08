using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Monkey : Entity
{
    private const string DyingTrigger = "IsDying";
    private float _timeBeforeDestroy = 2;

    public event Action<Monkey> Dying;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth <= 0)
            StartCoroutine(Die());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out BambooStick bambooStick))
        {
            if (CurrentHealth > 0)
                TakeDamage(bambooStick.Damage);
        }
    }

    private IEnumerator Die()
    {
        Animator.SetBool(DyingTrigger, true);

        yield return new WaitForSeconds(_timeBeforeDestroy);

        Dying?.Invoke(this);

        CurrentCoroutine = null;

        Destroy(gameObject);
    }
}
