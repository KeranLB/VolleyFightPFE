using UnityEngine;

public class UppercutZone : AttackZone
{
    public override Vector3 GetOutDirection()
    {
        return transform.forward + transform.up;
    }
}
