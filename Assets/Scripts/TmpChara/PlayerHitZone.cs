using UnityEngine;

public class PlayerHitZone : HitZone
{
    public Player player;

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        ChangeBallTeam(ball);
    }

    public override void OnOverlap(Ball ball)
    {
        ChangeBallTeam(ball);
    }

    private void ChangeBallTeam(Ball ball)
    {
        ball.ChangeTeam(player.team);
    }
}
