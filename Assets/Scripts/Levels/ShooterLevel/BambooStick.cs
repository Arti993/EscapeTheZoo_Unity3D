using UnityEngine;

public class BambooStick : Projectile
{
    public BambooStick()
    {
        MaxLifeTime = 5;
        PauseBeforeShutdown = 4;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Vector3 throwDirection = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

        Rigidbody.AddForce(throwDirection.normalized * Speed, ForceMode.VelocityChange);
    }
}
