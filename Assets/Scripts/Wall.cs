using System;
using UnityEngine;

public class Wall : HitZone
{
    public Vector3 normal;
    
    protected override void HitBall(Ball ball)
    {
        ball.Bounce(normal);
    }
}
