using UnityEngine;

public class BlockZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;

    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    
    private void Update()
    {
        if(player.playerInput.pressedBlock)
        {
            FlipActivation();
            player.playerInput.pressedBlock = false;
        }
    }
    
    private void FlipActivation()
    {
        _rigidbody.detectCollisions = !_rigidbody.detectCollisions;
        Color c;
        c = !_rigidbody.detectCollisions ? Color.red : Color.green;
        c.a = 0.3f;
        _meshRenderer.material.color = c;
    }
    
    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        base.OnTrajectory(ball, hitInfo);
        ball.Bunt(player);
        FlipActivation();
    }
}
