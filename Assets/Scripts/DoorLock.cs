using UnityEngine;
using UnityEngine.Events;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private GameObject _glow;
    [SerializeField] private AudioSource _openSound;

    public event UnityAction IsOpened;

    private void Start()
    {
        _glow.SetActive(false);
        Player.Instance.KeyIsTaken += OnKeyTaken;
    }

    private void OnKeyTaken()
    {
        _glow.SetActive(true);
        Player.Instance.KeyIsTaken -= OnKeyTaken;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out Player player))
        {
            if (player.IsKeyReceived)
            {
                IsOpened?.Invoke();

                _openSound.Play();

                Destroy(gameObject);
            }
        }
    }
}
