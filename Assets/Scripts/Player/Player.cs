using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonController))]
public class Player : Entity
{
    [SerializeField] private AudioSource _keyTakeSound;

    private ThirdPersonController _thirdPersonController;
    private float _pauseTimeAfterDeathBeforeRestart = 2;

    public static Player Instance;

    public event Action KeyIsTaken;
    public event Action Dying;

    public bool IsKeyReceived { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        IsKeyReceived = false;

        _thirdPersonController = GetComponent<ThirdPersonController>();
        
        base.Awake();
    }

    public void TakeKey()
    {
        _keyTakeSound.Play();
        IsKeyReceived = true;
        KeyIsTaken?.Invoke();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (CurrentHealth <= 0)
        {
            Dying?.Invoke();

            _thirdPersonController.DisableInput();

            StartCoroutine(WaitBeforeRestart());
        }
    }

    private IEnumerator WaitBeforeRestart()
    {
        yield return new WaitForSeconds(_pauseTimeAfterDeathBeforeRestart);

        LevelsChanger.Instance.CurrentLevel.Restart();
    }
}
