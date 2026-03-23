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
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnTrajectory(Ball ball, RaycastHit hitInfo)
    {
        if (_team != ball.teamPossess)
        {
            ChangeTeam(ball.teamPossess);
            _rigidbody.detectCollisions = false;
            ball.CheckCollisionAhead();
            _rigidbody.detectCollisions = true;
        }
        else
        {
            ball.Bounce(hitInfo);
        }
    }

    private Vector3 GetBounceNormal()
    {
        return (_team == Teams.TeamA ? _normalTeam1 : _normalTeam2).normalized;
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
