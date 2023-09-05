using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Fish : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    
    private AudioSource _playerHitSound;
    private Rigidbody _rigidbody;
    private FishSpawner _fishSpawner;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _fishSpawner = GetComponentInParent<FishSpawner>();
        _playerHitSound = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        var direction = _fishSpawner.transform.forward;

        _rigidbody.AddForce(direction * _speed, ForceMode.VelocityChange);

        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    private void Update()
    {
        if (_rigidbody.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out LakeBottom lakebottom))
        {
            ReturnToObjectPool();
        }

        if (other.gameObject.TryGetComponent(out ThirdPersonController _controller))
        {
            _controller.DisableInput();

            _playerHitSound.Play();
        }
    }

    private void ReturnToObjectPool()
    {
        gameObject.SetActive(false);
    }
}
