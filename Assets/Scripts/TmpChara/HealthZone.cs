using UnityEngine;

public class HealthZone : PlayerHitZone
{
    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        base.OnTrajectory(ball, hitInfo);
        ball.Bunt(player);
        player.playerLife.TakeDamage(2);
    }
}
