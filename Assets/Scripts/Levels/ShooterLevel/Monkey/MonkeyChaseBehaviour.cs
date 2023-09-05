using UnityEngine;
using UnityEngine.AI;

public class MonkeyChaseBehaviour : StateMachineBehaviour
{
    public const string AttackStateTrigger = "IsAttacking";

    private NavMeshAgent _agent;
    private float _attackRange = 10;
    private float _distance;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out NavMeshAgent agent))
            _agent = agent;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distance = Vector3.Distance(_agent.transform.position, Player.Instance.transform.position);

        _agent.SetDestination(Player.Instance.transform.position);

        if (_distance < _attackRange)
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
                    animator.SetBool(AttackStateTrigger, true);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _agent.SetDestination(_agent.transform.position);
    }
}
