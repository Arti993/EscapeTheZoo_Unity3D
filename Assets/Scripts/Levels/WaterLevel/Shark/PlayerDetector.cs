using System;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event Action PlayerDetected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            PlayerDetected?.Invoke();
        }
    }
}
