using UnityEngine;
using UnityEngine.AI;

public class MonkeyAttackBehaviour : StateMachineBehaviour
{
    public const string AttackStateTrigger = "IsAttacking";

    private float _maxAttackRange = 11;
    private BananaObjectPool _pool;
    private MonkeyRaycast _monkeyRaycast = new MonkeyRaycast();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out NavMeshAgent agent))
            agent.SetDestination(agent.transform.position);

        _pool = animator.gameObject.GetComponentInChildren<BananaObjectPool>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (DistanceMeterToPlayer.GetDistance(animator.gameObject) > _maxAttackRange)
            animator.SetBool(AttackStateTrigger, false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool isHit = _monkeyRaycast.TryGetHit(animator.gameObject, out RaycastHit hit);

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
