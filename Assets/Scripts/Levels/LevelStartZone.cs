using System;
using UnityEngine;

public class LevelStartZone : MonoBehaviour
{
    public event Action PlayerEntered;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            PlayerEntered?.Invoke();
        }
    }
}
