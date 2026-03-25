using UnityEngine;

public class HealthZone : PlayerHitZone
{
    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        if(ball.teamPossess != player.team)
        {
            base.OnTrajectory(ball, hitInfo);
            ball.Bunt(player);
            player.playerLife.TakeDamage(2);
        }
        else
        {
            _rigidbody.detectCollisions = false;
            ball.CheckCollisionAhead();
            _rigidbody.detectCollisions = true;
        }
    }
}
