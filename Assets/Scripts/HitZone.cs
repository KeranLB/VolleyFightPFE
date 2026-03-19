using UnityEngine;

public class HitZone : MonoBehaviour
{
    protected virtual void HitBall(Ball ball)
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            HitBall(ball);
        }
    }
}
