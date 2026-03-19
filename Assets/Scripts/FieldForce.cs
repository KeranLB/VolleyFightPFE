using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FieldForce : HitZone
{
    [SerializeField]
    private Teams _team;

    [SerializeField] private Vector3 _normalTeam1;
    [SerializeField] private Vector3 _normalTeam2;
    
    private MeshRenderer _meshRenderer;
    private Rigidbody _rigidbody;

    [SerializeField]
    private bool _letBallPass = true;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void HitBall(Ball ball)
    {
        if (_team != ball.teamPossess)
        {
            ChangeTeam(ball.teamPossess);
            _letBallPass = true;
        }
        else if(!_letBallPass)
        {
            ball.Bounce(_team==Teams.TeamA ? _normalTeam1 : _normalTeam2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            _letBallPass = false;
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
