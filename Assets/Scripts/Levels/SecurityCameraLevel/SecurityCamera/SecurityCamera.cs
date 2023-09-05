using UnityEngine;

public class SecurityCamera : MonoBehaviour 
{
    private float _rotateAmount = 0.45f;

	private void Update () 
    {
        float rotateDirectionY = Mathf.Sin(Time.realtimeSinceStartup);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotateDirectionY
            * _rotateAmount + transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
