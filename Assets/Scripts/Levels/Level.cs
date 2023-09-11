using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] protected ThirdPersonController PlayerController;
    [SerializeField] protected ExitLevelGates ExitLevelGates;
    [SerializeField] protected DoorLock ExitGatesDoorlock;
    [SerializeField] protected GameObject RestartPoint;
    [SerializeField] protected AudioSource MusicTheme;

    protected float DelayBeforeRestart = 2;

    private Coroutine _restartCoroutine;

    public event Action AllActionsFinished;
    public event Action Passed;
    public event Action Restarted;

    private void OnEnable()
    {
        ExitLevelGates.DoorsClosed += OnDoorsClosed;
        ExitGatesDoorlock.IsOpened += OnLevelPassed;
    }

    private void OnDisable()
    {
        ExitLevelGates.DoorsClosed -= OnDoorsClosed;
        ExitGatesDoorlock.IsOpened -= OnLevelPassed;
    }

    public void StartPlayMusic()
    {
        if (MusicTheme.isPlaying == false)
            MusicTheme.Play();
    }

    public void StopPlayMusic()
    {
        if (MusicTheme.isPlaying == true)
            MusicTheme.Stop();
    }

    public void Restart()
    {
        if (_restartCoroutine == null)
        {
            _restartCoroutine = StartCoroutine(WaitDelayAndRestart(DelayBeforeRestart));
        }
    }

    protected virtual void OnLevelPassed()
    {
        Passed?.Invoke();
    }

    protected virtual void OnDoorsClosed()
    {
        AllActionsFinished?.Invoke();
    }

    private IEnumerator WaitDelayAndRestart(float duration)
    {
        PlayerController.DisableInput();

        yield return new WaitForSeconds(duration);

        PlayerController.StopMovement();

        MovePlayerToRestartPosition();

        yield return new WaitForFixedUpdate();

        Restarted?.Invoke();

        PlayerController.StartMovement();

        PlayerController.EnableInput();

        if (Player.Instance.CurrentHealth <= 0)
            Player.Instance.RestoreFullHealth();

        _restartCoroutine = null;
    }

    private void MovePlayerToRestartPosition()
    {
        Vector3 restartPosition = RestartPoint.transform.position;

        PlayerController.transform.position = restartPosition;
    }
}