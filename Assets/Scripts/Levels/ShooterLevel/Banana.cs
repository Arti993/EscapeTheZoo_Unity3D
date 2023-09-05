using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Banana : Projectile
{
    private float _startPositionBiasFactor = 0.6f;

    public Banana()
    {
        MaxLifeTime = 5;
        PauseBeforeShutdown = 4;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Transform target = Player.Instance.gameObject.transform;

        target.position = new Vector3(target.position.x, target.position.y + _startPositionBiasFactor, target.position.z);

        transform.LookAt(target);

        Rigidbody.AddForce(transform.forward * Speed, ForceMode.VelocityChange);

        target.position = new Vector3(target.position.x, target.position.y - _startPositionBiasFactor, target.position.z);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            if (player.CurrentHealth > 0)
                player.TakeDamage(this.Damage);
        }

        base.OnCollisionEnter(collision);
    }
}
