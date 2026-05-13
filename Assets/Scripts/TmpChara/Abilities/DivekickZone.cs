using UnityEngine;

public class DivekickZone : AttackZone
{
    public override Vector3 GetOutDirection()
    {
        return transform.forward + -transform.up;
    }
}
