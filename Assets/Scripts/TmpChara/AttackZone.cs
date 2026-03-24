using UnityEngine;

public class AttackZone : PlayerHitZone
{
    private MeshRenderer _meshRenderer;
    
    protected override void Awake()
    {
        base.Awake();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FlipActivation();
        }
    }

    private void FlipActivation()
    {
        _rigidbody.detectCollisions = !_rigidbody.detectCollisions;
        if (!_rigidbody.detectCollisions)
        {
            _meshRenderer.material.color = Color.red;
        }
        else
        {
            _meshRenderer.material.color = Color.blue;
        }
    }

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        base.OnTrajectory(ball, hitInfo);
        Vector3 hitDirection = (
            transform.right * player.playerInput.currentInput.x
            + transform.up * player.playerInput.currentInput.y
            + transform.forward
        );
        ball.StopSimulation(hitInfo.point);
        ball.GetHit(hitDirection);
        FlipActivation();
    }
}
