using UnityEngine;

public class GuardZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;
    public int speedLevelOverride;
    private bool _hasTouchedBall;
    
    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        _rigidbody.detectCollisions = true;
        Color c = Color.blue;
        c.a = 0.5f;
        _meshRenderer.material.color = c;
        _hasTouchedBall = false;
    }

    public void Deactivate()
    {
        _rigidbody.detectCollisions = false;
        Color c = Color.red;
        c.a = 0.3f;
        _meshRenderer.material.color = c; 
    }

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        ball.ChangeSpeedLevel(speedLevelOverride);
        ball.Bunt(player, hitInfo.point);
        base.OnTrajectory(ball, hitInfo);
    }

    public override void OnOverlap(Ball ball)
    {
        ball.ChangeSpeedLevel(speedLevelOverride);
        ball.Bunt(player, ball.transform.position);
        base.OnOverlap(ball);
    }
}
