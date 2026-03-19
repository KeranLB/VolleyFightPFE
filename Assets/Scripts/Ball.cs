using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Ball : MonoBehaviour
{
    #region Speeds

    [SerializeField] private float _maxSpeed;
    private float _realSpeed;
    private float _hitSpeeed;
    [SerializeField] private float _passSpeed;

    #endregion

    #region Directions

    private Vector3 _direction;
    private Vector3 _target;

    #endregion

    #region Possession

    public Teams teamPossess;
    private int _touchCount = 0;
    [SerializeField] private int _touchLimit;
    [SerializeField] private int _timeLimit;
    private float _timer;
    
    #endregion
    
    #region Subcomponents
        private Rigidbody _rigidbody;
        private MeshRenderer _meshRenderer;
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _maxSpeed += 5;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            _maxSpeed -= 5;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
           GetHit(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GetHit(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            GetHit(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            GetHit(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GetHit(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            GetHit(Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GetHit(Vector3.up*2 + Vector3.forward + Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            GetHit(Vector3.up + Vector3.forward*3 + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GetHit(Vector3.down + Vector3.forward + Vector3.left*2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GetHit(Vector3.down*2.5f + Vector3.forward + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            ChangeTeam(teamPossess==Teams.TeamA ? Teams.TeamB : Teams.TeamA);
        }
    }

    private void FixedUpdate()
    {
        CheckCollisionAhead(transform.position);
    }

    public void CheckCollisionAhead(Vector3 startPoint)
    {
        if(
            Physics.Raycast(startPoint, _direction, out RaycastHit hit, _realSpeed * Time.fixedDeltaTime)
            && hit.collider.TryGetComponent<HitZone>(out HitZone zone)
        )
        {
            zone.OnTrajectory(this, hit);
        }
        else
        {
            MoveInDirection();
        }
    }

    public void StopBeforeCollision(RaycastHit hit)
    {
        _rigidbody.MovePosition(hit.point - _direction * 0.5f);
    }

    public void MoveInDirection()
    {
        _rigidbody.MovePosition(transform.position + _realSpeed * Time.fixedDeltaTime * _direction);
    }

    public void ChangeTeam(Teams team)
    {
        teamPossess = team;
        if (team == Teams.TeamA)
        {
            _meshRenderer.material.color = Color.blue;
        }
        else if (team == Teams.TeamB)
        {
            _meshRenderer.material.color = Color.yellow;
        }
    }

    void PhysicSim()
    {
        
    }

    public void Bounce(Vector3 normal)
    {
        _direction = Vector3.Reflect(_direction, normal.normalized);
        _realSpeed = _maxSpeed;
    }

    void GetHit(Vector3 direction, float accSpeed = 1)
    {
        _direction = direction.normalized;
        _maxSpeed *= accSpeed;
        _realSpeed = _maxSpeed;
    }

    void GetBlocked(Vector3 target)
    {
        _target = target;
        _realSpeed = _passSpeed;
    }

    void CrossField()
    {
        StopCoroutine(Touch());
        _touchCount = 0;
    }

    void NewTouch()
    {
        _touchCount++;
        if (_touchLimit >= _touchCount)
        {
            TouchLimit();
        }
    }
    
    void TouchLimit()
    {
        //conditions if touch limit if cross
    }

    IEnumerator Touch()
    {
        yield return new WaitForSeconds(1f);
        _timer++;
        if (_timer >= _timeLimit)
        {
            TimeLimit();
        }
        else
        {
            StartCoroutine(Touch());
        }
    }

    void TimeLimit()
    {
        // conditions if time is over
    }
    
}
