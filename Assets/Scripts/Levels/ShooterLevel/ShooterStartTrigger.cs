using UnityEngine;
using StarterAssets;

public class ShooterStartTrigger : MonoBehaviour
{
    [SerializeField] private ShooterController _shooterController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonController controller))
        {
            controller.EnableShooting();

            _shooterController.StartWork();

            this.gameObject.SetActive(false);
        }
    }
}
