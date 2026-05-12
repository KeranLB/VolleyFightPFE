using System;
using UnityEngine;

public class AttackZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;
    public int speedLevelChange;

    public static event Action<int> OnPlayerAttackSuccess;


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
        base.OnTrajectory(ball, hitInfo);
        Vector3 hitDirection = (
            transform.right * player.playerInput.currentDirection.x
            + transform.up * player.playerInput.currentDirection.y
            + transform.forward
        );
        ball.StopSimulation(hitInfo.point);
        ball.GetHit(hitDirection, speedLevelChange);
        OnPlayerAttackSuccess?.Invoke(player.gameObject.GetInstanceID());
        player.playerActions.CancelAction();
    }
}
