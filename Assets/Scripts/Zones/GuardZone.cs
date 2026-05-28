using UnityEngine;

public class GuardZone : AttackZone
{
    public int speedLevelOverride;
    public override Vector3 GetOutDirection()
    {
        return transform.up;
    }

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        ball.ChangeSpeedLevel(speedLevelOverride);
        base.OnTrajectory(ball, hitInfo);
    }

    public override void OnOverlap(Ball ball)
    {
        ball.ChangeSpeedLevel(speedLevelOverride);
        base.OnOverlap(ball);
    }
}
