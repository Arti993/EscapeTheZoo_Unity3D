using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Shark : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private PlayerDetector _playerDetector;
    [SerializeField] private AudioSource _biteSound;

    private Vector3 _direction;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private float _attackSpeed = 4;
    private float _attackJumpForce = 5;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _direction = transform.forward;

        _rigidbody.AddForce(_direction.normalized * _moveSpeed, ForceMode.VelocityChange);

        _playerDetector.PlayerDetected += OnPlayerDetected;
    }

    private void OnDisable()
    {
        _playerDetector.PlayerDetected -= OnPlayerDetected;
    }

    private void OnCollisionEnter(Collision collision)
    {
            transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
            Vector3 normalizedVelocity = _rigidbody.velocity.normalized;
            _rigidbody.velocity = normalizedVelocity * _moveSpeed;
    }

    private void OnPlayerDetected()
    {
        _rigidbody.velocity = Vector3.zero;
        _collider.isTrigger = true;
        _rigidbody.useGravity = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        transform.LookAt(Player.Instance.transform.position);

        _rigidbody.AddForce(Vector3.up * _attackJumpForce, ForceMode.Impulse);

        _rigidbody.AddForce(transform.forward * _attackSpeed, ForceMode.Impulse);

        _playerDetector.PlayerDetected -= OnPlayerDetected;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ThirdPersonController controller))
        {
            if(controller.IsPlayerHitByShark == false)
            {
                StartCoroutine(controller.MoveAfterSharkHit(this));

                _biteSound.Play();
            }
        }

        if (other.gameObject.TryGetComponent(out LakeBottom lakeBottom))
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            transform.position = _startPosition;
            transform.rotation = _startRotation;
            _direction = transform.forward;
            _collider.isTrigger = false;
            _rigidbody.useGravity = false;

            _rigidbody.AddForce(_direction.normalized * _moveSpeed, ForceMode.VelocityChange);

            _playerDetector.PlayerDetected += OnPlayerDetected;
        }
    }
}
