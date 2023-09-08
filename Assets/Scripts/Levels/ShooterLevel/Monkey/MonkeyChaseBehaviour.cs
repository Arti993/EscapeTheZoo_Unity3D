using UnityEngine;
using UnityEngine.AI;

public class MonkeyChaseBehaviour : StateMachineBehaviour
{
    public const string AttackStateTrigger = "IsAttacking";

    private NavMeshAgent _agent;
    private float _minAttackRange = 10;
    private MonkeyRaycast _monkeyRaycast = new MonkeyRaycast();
 
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out NavMeshAgent agent))
            _agent = agent;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _agent.SetDestination(Player.Instance.transform.position);

        if (DistanceMeterToPlayer.GetDistance(animator.gameObject) < _minAttackRange)
        {
            bool isHit = _monkeyRaycast.TryGetHit(animator.gameObject, out RaycastHit hit);

            if (isHit)
            {
                if (hit.collider.gameObject.TryGetComponent(out Player player))
                    animator.SetBool(AttackStateTrigger, true);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _agent.SetDestination(_agent.transform.position);
    }
}
