using UnityEngine;

public class HealthZone : PlayerHitZone
{
    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        if(ball.teamPossess != player.team)
        {
            Vector3 position = player.transform.position;
            base.OnTrajectory(ball, hitInfo);
            player.playerLife.TakeDamage(ball.GetFinalDamage());
            ball.ChangeSpeedLevel(1);
            ball.Bunt(player, position);
        }
        else
        {
            _rigidbody.detectCollisions = false;
            ball.CheckCollisionAhead();
            _rigidbody.detectCollisions = true;
        }
    }

    public override void OnOverlap(Ball ball)
    {
        if(ball.teamPossess != player.team)
        {
            Vector3 position = player.transform.position;
            base.OnOverlap(ball);
            player.playerLife.TakeDamage(ball.GetFinalDamage());
            ball.ChangeSpeedLevel(1);
            ball.Bunt(player, position);
        }
        else
        {
            _rigidbody.detectCollisions = false;
            ball.CheckCollisionAhead();
            _rigidbody.detectCollisions = true;
        }
    }
}
