using System;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    [SerializeField] private AudioSource _fallSound;

    public Quaternion DefaultRotation { get; private set; }

    public event Action BorderHitted;

    private void Start()
    {
        transform.parent = null;
        DefaultRotation = transform.rotation;
        LevelsChanger.Instance.CurrentLevel.AllActionsFinished += OnLevelConfirmed;
    }

    private void OnLevelConfirmed()
    {
        LevelsChanger.Instance.CurrentLevel.AllActionsFinished -= OnLevelConfirmed;

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var levelBorders = collision.gameObject.GetComponentInParent<LevelBorders>();

        bool isCollisionWithLevelBorders = levelBorders != null;

        if (isCollisionWithLevelBorders)
        {
            BorderHitted?.Invoke();
        }

        bool isCollisionWithPlayer = collision.gameObject.TryGetComponent(out Player player);

        bool isNeedToPlayFallSound = isCollisionWithPlayer == false && isCollisionWithLevelBorders == false;

        if (isNeedToPlayFallSound)
            _fallSound.Play();
    }
}
