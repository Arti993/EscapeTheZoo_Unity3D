using UnityEngine;
using StarterAssets;

public class ShooterStartTrigger : MonoBehaviour
{
    [SerializeField] private ShooterController _shooterController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonController controller))
        {
            ThirdPersonController.Instance.EnableShooting();

            _shooterController.gameObject.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }
}
