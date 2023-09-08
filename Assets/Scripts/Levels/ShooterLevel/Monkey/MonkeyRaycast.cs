using UnityEngine;

public class MonkeyRaycast : MonoBehaviour
{
    private float _maxDistance = 20;
    private float _directionVectorMultiplier = 0.5f;
    private float _startPositionBiasFactor = 0.6f;

    public bool TryGetHit(GameObject monkey, out RaycastHit hit)
    {
        monkey.transform.LookAt(Player.Instance.gameObject.transform);

        Vector3 raycastDirection = monkey.transform.forward;
        Vector3 auxiliaryPoint = monkey.transform.position + raycastDirection * _directionVectorMultiplier;
        Vector3 raycastStartPoint = new Vector3(auxiliaryPoint.x,
            auxiliaryPoint.y + _startPositionBiasFactor, auxiliaryPoint.z);

        bool isHit = Physics.Raycast(raycastStartPoint, raycastDirection, out RaycastHit newhit, _maxDistance);

        hit = newhit;

        return isHit;
    }
}
