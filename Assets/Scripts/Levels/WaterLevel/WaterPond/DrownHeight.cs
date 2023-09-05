using UnityEngine;
using StarterAssets;

public class DrownHeight : MonoBehaviour
{
    [SerializeField] private AudioSource _fallingIntoWater;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonController _controller))
        {
            _controller.DrownInWater();

            _fallingIntoWater.Play();
        }
    }
}
