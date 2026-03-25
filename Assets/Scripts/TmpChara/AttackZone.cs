using UnityEngine;

public class AttackZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;
    public float speedMult;
    
    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if(player.playerInput.pressedAttack)
        {
            FlipActivation();
            player.playerInput.pressedAttack = false;
        }
    }

    private void FlipActivation()
    {
        _rigidbody.detectCollisions = !_rigidbody.detectCollisions;
        Color c;
        c = !_rigidbody.detectCollisions ? Color.red : Color.blue;
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
        ball.GetHit(hitDirection, speedMult);
        FlipActivation();
    }
}
