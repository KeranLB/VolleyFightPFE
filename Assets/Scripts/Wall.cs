using System;
using UnityEngine;

public class Wall : HitZone
{
    public Vector3 normal;

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        // ball.StopBeforeCollision(hitInfo, normal.normalized);
        ball.Bounce(hitInfo);
    }
}
