using UnityEngine;
using UnityEngine.AI;

public class MonkeyAttackBehaviour : StateMachineBehaviour
{
    public const string AttackStateTrigger = "IsAttacking";

    private float _distance;
    private BananaObjectPool _pool;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out NavMeshAgent agent))
            agent.SetDestination(agent.transform.position);

        _pool = animator.gameObject.GetComponentInChildren<BananaObjectPool>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distance = Vector3.Distance(animator.transform.position, Player.Instance.transform.position);

        if (_distance > 11)
            animator.SetBool(AttackStateTrigger, false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.LookAt(Player.Instance.gameObject.transform);

        Vector3 raycastDirection = animator.transform.forward;
        Vector3 auxiliaryPoint = animator.transform.position + raycastDirection * 0.5f;
        Vector3 raycastStartPoint = new Vector3(auxiliaryPoint.x, auxiliaryPoint.y + 0.6f, auxiliaryPoint.z);

        RaycastHit hit;
        bool isHit = Physics.Raycast(raycastStartPoint, raycastDirection, out hit, 20);

        if (isHit)
        {
            if (hit.collider.gameObject.TryGetComponent(out Player player))
            {
                if (_pool != null)
                    _pool.TryGetBanana();
            }
            else if (hit.collider.gameObject.TryGetComponent(out Banana banana))
            {
                return;
            }
            else
            {
                animator.SetBool(AttackStateTrigger, false);

                _pool.ResetFirstPause();
            }
        }
    }
}
