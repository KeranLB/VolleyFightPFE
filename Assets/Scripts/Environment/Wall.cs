using UnityEngine;

public class Wall : HitZone
{
    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        ball.Bounce(hitInfo);
    }

    public override void OnOverlap(Ball ball)
    {
        ball.Bounce(ball.transform.position, transform.forward, 0);
    }
}
