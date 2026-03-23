using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FieldForce : HitZone
{
    [SerializeField]
    private Teams _team;
    
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;

    [SerializeField]
    private bool _letBallPass = true;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        if (_team != ball.teamPossess)
        {
            // Switch sides and let the ball through
            ChangeTeam(ball.teamPossess);
            _rigidbody.detectCollisions = false;
            ball.CheckCollisionAhead();
            ball.isSwitchingSide = true;
            _rigidbody.detectCollisions = true;
        }
        else if(ball.isSwitchingSide)
        {
            // Let the ball through
            _rigidbody.detectCollisions = false;
            ball.CheckCollisionAhead();
            _rigidbody.detectCollisions = true;
        }
        else
        {
            ball.Bounce(hitInfo);
        }
    }

    private void ChangeTeam(Teams team)
    {
        _team = team;
        if (team == Teams.TeamA)
        {
            _meshRenderer.material.color = Color.blue;
        }
        else if (team == Teams.TeamB)
        {
            _meshRenderer.material.color = Color.yellow;
        }
    }
}
