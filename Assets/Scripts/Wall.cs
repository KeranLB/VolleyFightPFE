using UnityEngine;

public class Wall : HitZone
{
    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        ball.Bounce(hitInfo);
    }
}
