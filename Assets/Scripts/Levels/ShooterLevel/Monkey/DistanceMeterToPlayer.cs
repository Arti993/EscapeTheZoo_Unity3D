using UnityEngine;

public class DistanceMeterToPlayer
{
    public static float GetDistance(GameObject monkey)
    {
        return Vector3.Distance(monkey.transform.position, Player.Instance.transform.position);
    }
}
