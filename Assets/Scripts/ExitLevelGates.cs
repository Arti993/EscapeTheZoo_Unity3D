using System;
using System.Collections;
using UnityEngine;

public class ExitLevelGates : MonoBehaviour
{
    [SerializeField] private DoorLock _doorlock;
    [SerializeField] private GameObject _leftDoor;
    [SerializeField] private GameObject _rightDoor;
    [SerializeField] private GameObject _doorDeadbolt;
    [SerializeField] private LevelStartZone _nextLevelStartZone;
    [SerializeField] private float _doorsOpenSpeed;

    private float _doorsCloseSpeed;
    private float _doorsCloseSpeedModificator = 3;
    private float _leftDoorOpenedRotationY = -90;
    private float _rightDoorOpenedRotationY = 90;
    private float _leftDoorClosedRotationY = 0;
    private float _rightDoorClosedRotationY = 0;

    public event Action DoorsClosed;

    private void OnEnable()
    {
        _doorsCloseSpeed = _doorsOpenSpeed * _doorsCloseSpeedModificator;
        _doorlock.IsOpened += OnDoorLockOpen;
        _nextLevelStartZone.PlayerEntered += OnNextLevelStarted;
    }

    private void OnDisable()
    {
        _doorlock.IsOpened -= OnDoorLockOpen;
        _nextLevelStartZone.PlayerEntered -= OnNextLevelStarted;
    }

    private void OnDoorLockOpen()
    {
        _doorDeadbolt.SetActive(false);
        StartCoroutine(OpenDoors());
    }

    private void OnNextLevelStarted()
    {
        _nextLevelStartZone.gameObject.SetActive(false);
        StartCoroutine(CloseDoors());
    }

    private IEnumerator OpenDoors()
    {
        float rightDoorRotationY = _rightDoorClosedRotationY;
        float leftDoorRotationY = _leftDoorClosedRotationY;

        while (rightDoorRotationY != _rightDoorOpenedRotationY
            && leftDoorRotationY != _leftDoorOpenedRotationY)
        {
            rightDoorRotationY = Mathf.MoveTowards(rightDoorRotationY, _rightDoorOpenedRotationY,
                _doorsOpenSpeed * Time.deltaTime);

            _rightDoor.transform.rotation = Quaternion.Euler(0, rightDoorRotationY, 0);

            leftDoorRotationY = Mathf.MoveTowards(leftDoorRotationY, _leftDoorOpenedRotationY,
                _doorsOpenSpeed * Time.deltaTime);

            _leftDoor.transform.rotation = Quaternion.Euler(0, leftDoorRotationY, 0);

            yield return null;
        }
    }

    private IEnumerator CloseDoors()
    {
        float rightDoorRotationY = _rightDoorOpenedRotationY;
        float leftDoorRotationY = _leftDoorOpenedRotationY;

        while (rightDoorRotationY != _rightDoorClosedRotationY
            && leftDoorRotationY != _leftDoorClosedRotationY)
        {
            rightDoorRotationY = Mathf.MoveTowards(rightDoorRotationY, _rightDoorClosedRotationY,
                _doorsCloseSpeed * Time.deltaTime);

            _rightDoor.transform.rotation = Quaternion.Euler(0, rightDoorRotationY, 0);

            leftDoorRotationY = Mathf.MoveTowards(leftDoorRotationY, _leftDoorClosedRotationY,
                _doorsCloseSpeed * Time.deltaTime);

            _leftDoor.transform.rotation = Quaternion.Euler(0, leftDoorRotationY, 0);

            yield return null;
        }

        _doorDeadbolt.SetActive(true);

        DoorsClosed?.Invoke();
    }
}
