using UnityEngine;
using StarterAssets;

public class ShooterStartTrigger : MonoBehaviour
{
    [SerializeField] private ShooterWavesController _shooterWavesController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonController controller))
        {
            ThirdPersonController.Instance.EnableShooting();

            _shooterWavesController.gameObject.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }
}
