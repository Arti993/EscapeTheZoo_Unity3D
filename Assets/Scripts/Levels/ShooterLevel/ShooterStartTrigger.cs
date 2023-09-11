using UnityEngine;
using StarterAssets;

public class ShooterStartTrigger : MonoBehaviour
{
    [SerializeField] private MonkeyWavesSpawner _monkeyWavesSpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonController controller))
        {
            ThirdPersonController.Instance.EnableShooting();

            _monkeyWavesSpawner.gameObject.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }
}
