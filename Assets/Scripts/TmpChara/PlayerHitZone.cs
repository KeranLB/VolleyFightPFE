using UnityEngine;

public class PlayerHitZone : HitZone
{
    public Player player;

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        ChangeBallTeam(ball);
    }

    private void ChangeBallTeam(Ball ball)
    {
        ball.ChangeTeam(player.team);
    }
}
