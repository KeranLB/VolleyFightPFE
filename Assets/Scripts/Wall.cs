using System;
using UnityEngine;

public class Wall : HitZone
{
    public Vector3 normal;
    void HitBall(Ball ball)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        Debug.Log("Hit");
        Debug.Log("Hit");
        Debug.Log("Hit");
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            ball.Bounce(normal);
        }
    }
}
