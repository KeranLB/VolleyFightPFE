using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FieldForce : HitZone
{
    [SerializeField]
    private Teams _team;
    
    private MeshRenderer _meshRenderer;

    [SerializeField]
    private bool _letBallPass = true;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
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

    public void ChangeTeam(Teams team)
    {
        _team = team;
        Color c = team switch
        {
            Teams.TeamA => Color.blue,
            Teams.TeamB => Color.yellow,
            Teams.Neutral => Color.green,
        };
        c.a = 0.5f;
        _meshRenderer.material.color = c;
    }
}
