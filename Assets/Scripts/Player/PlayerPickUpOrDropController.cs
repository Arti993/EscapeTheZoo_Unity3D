using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerPickUpOrDropController : MonoBehaviour
{
    [SerializeField] private Button _PickUpOrDropButton;
    [SerializeField] private AudioSource _PickUpSound;

    private PickableObject _currentTakenItem;

    private bool _isHolding = false;
    private float _takeDistance = 0.7f;
    private Vector3 _holdDistanceVector = new Vector3(0, 0.5f, 0.5f);

    public event Action ItemTaken;
    public event Action ItemDropped;

    public void OnPickUpOrDrop()
    {
        if(ThirdPersonController.Instance.IsPickUpOrDropEnable)
        {
            if (_isHolding == false)
            {
                if (TryPickUpItem())
                {
                    _isHolding = true;
                    ItemTaken?.Invoke();
                    _PickUpSound.Play();
                    _currentTakenItem.BorderHitted += OnTakenItemBorderHit;
                }
            }
            else
            {
                DropTakenItem();
            }
        }
    }

    public void EnablePickUpOrDropButton()
    {
        _PickUpOrDropButton.gameObject.SetActive(true);
    }

    public void DisablePickUpOrDropButton()
    {
        _PickUpOrDropButton.gameObject.SetActive(false);
    }

    private void OnTakenItemBorderHit()
    {
        DropTakenItem();
    }

    private bool TryPickUpItem()
    {
        bool isPickUpSuccessful = false;

        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 
            _takeDistance) && hitInfo.collider.gameObject.TryGetComponent(out PickableObject takenItem))
        {
            _currentTakenItem = takenItem;

            _currentTakenItem.transform.rotation = _currentTakenItem.DefaultRotation;

            _currentTakenItem.transform.position = default;

            _currentTakenItem.transform.SetParent(transform, worldPositionStays: false);

            _currentTakenItem.transform.localPosition += _holdDistanceVector;

            var takenItemRigidbody = _currentTakenItem.GetComponent<Rigidbody>();

            takenItemRigidbody.useGravity = false;

            takenItemRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            isPickUpSuccessful = true;
        }

        return isPickUpSuccessful;
    }

    private void DropTakenItem()
    {
        _currentTakenItem.transform.parent = null;

        var takenItemRigidbody = _currentTakenItem.GetComponent<Rigidbody>();

        takenItemRigidbody.useGravity = true;

        takenItemRigidbody.constraints = RigidbodyConstraints.None;

        _isHolding = false;

        ItemDropped?.Invoke();

        _currentTakenItem.BorderHitted -= OnTakenItemBorderHit;
    }
}
