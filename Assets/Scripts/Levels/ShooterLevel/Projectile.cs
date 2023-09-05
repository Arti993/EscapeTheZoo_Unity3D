using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;

    protected Rigidbody Rigidbody;
    protected ObjectPool Pool;
    protected float MaxLifeTime;
    protected float PauseBeforeShutdown;

    private float _currentLifeTime;
    private bool _isReadyToShutdown = false;
    private Quaternion _defaultRotation;
    private Coroutine _existEndingCoroutine;

    public float Damage => _damage;

    public float Speed => _speed;

    protected virtual void OnEnable()
    {
        _currentLifeTime = 0;
        transform.parent = null;
        _isReadyToShutdown = false;

        StartCoroutine(ExistUntilLifetimeEnd());
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (_isReadyToShutdown == false)
        {
            EndExistanceIfPossible();

            _isReadyToShutdown = true;
        }
    }

    private void Awake()
    {
        Pool = GetComponentInParent<ObjectPool>();
        Rigidbody = GetComponent<Rigidbody>();
        _defaultRotation = transform.rotation;
    }

    private void EndExistanceIfPossible()
    {
        if (_existEndingCoroutine == null)
            _existEndingCoroutine = StartCoroutine(EndExistance());
    }

    private IEnumerator ExistUntilLifetimeEnd()
    {
        while (_currentLifeTime < MaxLifeTime)
        {
            _currentLifeTime += Time.deltaTime;

            yield return null;
        }

        EndExistanceIfPossible();
    }

    private IEnumerator EndExistance()
    {
        Rigidbody.useGravity = true;

        yield return new WaitForSeconds(PauseBeforeShutdown);

        Rigidbody.useGravity = false;

        _existEndingCoroutine = null;

        if (Pool != null && Pool.gameObject.activeInHierarchy)
            ReturnToObjectPool();
        else
            Destroy(gameObject);
    }

    private void ReturnToObjectPool()
    {
        Rigidbody.velocity = Vector3.zero;

        transform.SetParent(Pool.transform);

        transform.rotation = _defaultRotation;

        gameObject.SetActive(false);
    }
}
