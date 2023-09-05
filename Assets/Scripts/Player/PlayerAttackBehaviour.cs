using UnityEngine;
using StarterAssets;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    private const string ThrowState = "IsThrow";

    private BambooObjectPool _pool;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _pool = animator.gameObject.GetComponentInChildren<BambooObjectPool>();

        if (animator.gameObject.TryGetComponent(out ThirdPersonController _controller))
        {
            _controller.DisableInput();
            _controller.StopMovement();
            _controller.TurnOnControlAfterFixedUpdate();
        }

        Vector3 throwDirection = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z);

        animator.gameObject.transform.rotation = Quaternion.LookRotation(throwDirection);

        if (_pool != null)
            _pool.TryGetBamboo();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(ThrowState, false);
    }
}
