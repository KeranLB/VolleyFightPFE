using UnityEngine;

public class HitZone : MonoBehaviour
{
    protected virtual void HitBall(Ball ball)
    {
        
    }

    public virtual void OnTrajectory(Ball ball, RaycastHit hitInfo)
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
