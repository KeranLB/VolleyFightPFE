using System;
using UnityEngine;

public class AttackZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;
    public int speedLevelChange;
    private bool _hasTouchedBall;

    public event Action<Ball, bool> OnPlayerAttackSuccess;


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

    public virtual Vector3 GetOutDirection()
    {
        Vector3 hitDirection = (
            transform.right * player.playerInput.currentDirection.x
            + transform.up * player.playerInput.currentDirection.y
            + transform.forward
        );

        return hitDirection;
    }

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        // Ignore subsequent hits for same activation
        if (_hasTouchedBall)
        {
            ball.PassThrough(hitInfo);
            return;
        }

        _hasTouchedBall = true;
        OnPlayerAttackSuccess?.Invoke(ball, false);
        base.OnTrajectory(ball, hitInfo);
        ball.StopSimulation(hitInfo.point);
        ball.GetHit(GetOutDirection(), speedLevelChange, player);
        ball.Freeze();
        player.playerAim.StartAim(ball, GetOutDirection());
    }

    public override void OnOverlap(Ball ball)
    {
        // Ignore subsequent hits for same activation
        if (_hasTouchedBall)
        {
            return;
        }
        
        _hasTouchedBall = true;
        OnPlayerAttackSuccess?.Invoke(ball, true);
        base.OnOverlap(ball);
        ball.GetHit(GetOutDirection(), speedLevelChange, player);
        ball.Freeze();
        player.playerAim.StartAim(ball, GetOutDirection());
    }
}
