using UnityEngine;

public class DivekickZone : AttackZone
{
    public override Vector3 GetOutDirection()
    {
        if(player.playerMovement.isGrounded)
            return transform.forward;
        return transform.forward + -transform.up;
    }
}
