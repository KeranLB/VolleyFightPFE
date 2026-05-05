using System;
using UnityEngine;

public class BlockZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;

    public static event Action<int> OnPlayerBlockSuccess;

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
        Color c = Color.green;
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
        ball.Bunt(player);
        OnPlayerBlockSuccess?.Invoke(player.gameObject.GetInstanceID());
        player.playerActions.CancelAction();
    }
}
