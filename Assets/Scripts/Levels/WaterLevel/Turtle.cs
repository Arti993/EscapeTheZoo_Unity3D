using System.Collections;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    [SerializeField] AudioSource _reboundSound;

    private Vector3 _startPosition;
    private float _collisionsWithPlayerCount = 0f;
    private float _maxCollisionsWithPlayerBeforeDive = 3f;
    private float _collisionWithPlayerDetectTime = 0.5f;
    private float _divingDepth = 0.8f;
    private float _diveTime = 3.5f;
    private float _currentDivingTime = 0f;
    private Coroutine _collisionsWithPlayerCountCoroutine;

    public void CollisionWithPlayer()
    {
        if (_collisionsWithPlayerCountCoroutine == null)
        {
            _reboundSound.Play();

            _collisionsWithPlayerCountCoroutine = StartCoroutine(CountCollisionsWithPlayer());
        }

    }

    private void OnEnable()
    {
        _startPosition = transform.position;

        LevelsChanger.Instance.CurrentLevel.Restarted += OnLevelRestarted;
    }

    private void OnDisable()
    {
        LevelsChanger.Instance.CurrentLevel.Restarted -= OnLevelRestarted;
    }

    private void Dive()
    {
        _currentDivingTime = 0;
        StartCoroutine(DiveAndRise());
    }

    private IEnumerator DiveAndRise()
    {
        Vector3 destinationPoint = new Vector3(transform.position.x, transform.position.y - _divingDepth,
            transform.position.z);

        while (_diveTime > _currentDivingTime)
        {
            transform.position = Vector3.Lerp(transform.position, destinationPoint, _currentDivingTime / _diveTime);
            _currentDivingTime += Time.deltaTime;
            yield return null;
        }

        _currentDivingTime = 0;

        while (_diveTime > _currentDivingTime)
        {
            transform.position = Vector3.Lerp(transform.position, _startPosition, _currentDivingTime / _diveTime);
            _currentDivingTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator CountCollisionsWithPlayer()
    {
        _collisionsWithPlayerCount++;

        if (_maxCollisionsWithPlayerBeforeDive == _collisionsWithPlayerCount)
        {
            Dive();
            _collisionsWithPlayerCount = 0;
        }

        yield return new WaitForSeconds(_collisionWithPlayerDetectTime);

        _collisionsWithPlayerCountCoroutine = null;
    }

    private void OnLevelRestarted()
    {
        _collisionsWithPlayerCount = 0;
    }
}
