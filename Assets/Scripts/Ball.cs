using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
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
    public bool isSwitchingSide = false;
    
    #endregion
    
    #region Subcomponents
        private Rigidbody _rigidbody;
        private MeshRenderer _meshRenderer;
        private Collider _collider;
    #endregion

    #region Physics

    private Vector3 _currentFramePosition;
    private Vector3 _currentFrameDirection;
    private float _currentFrameDistanceRemaining;
    private const float MAX_STEP_LENGTH = 0.5f;
    
    //DEBUG
    [SerializeField]
    private List<Vector3> _currentFrameCollisions;
    private Vector3 _currentFrameDestination;

    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        
        _currentFramePosition = _rigidbody.position;
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
        _rigidbody.MovePosition(_currentFramePosition);
        _currentFrameCollisions.Clear();
        _currentFrameDirection = _direction.normalized;
        _currentFrameDistanceRemaining = _realSpeed * Time.fixedDeltaTime;
        
        // Check we're not already overlapping a collider that would be ignored by SphereCast
        Collider[] cols = Physics.OverlapSphere(_currentFramePosition, 0.5f);
        foreach (Collider collider in cols)
        {
            if(
                Physics.Raycast(_currentFramePosition, _currentFrameDirection, out RaycastHit hit, Mathf.Min(_currentFrameDistanceRemaining, 0.5f))
                && hit.collider.TryGetComponent<HitZone>(out HitZone hitZone))
            {
                hitZone.OnTrajectory(this, hit);
            }
        }
        
        // Simulate next position until all collisions are resolved
        int hardLimit = 100; // Don't get stuck in infinite loop
        while (_currentFrameDistanceRemaining > 0 && hardLimit > 0)
        {
            CheckCollisionAhead();
            hardLimit--;
        }

        if (hardLimit == 0)
        {
            Debug.LogError("HARD LIMIT REACHED");
        }
        _direction = _currentFrameDirection;
        _currentFrameDestination = _currentFramePosition;
    }

    public void CheckCollisionAhead()
    {
        if(
            Physics.SphereCast(_currentFramePosition, 0.5f, _currentFrameDirection, out RaycastHit hit, Mathf.Min(_currentFrameDistanceRemaining, MAX_STEP_LENGTH))
            //Physics.Raycast(_currentFramePosition, _currentFrameDirection, out RaycastHit hit, _currentFrameDistanceRemaining)
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

    public void ReduceFrameDistanceRemaining(float amount)
    {
        _currentFrameDistanceRemaining -= amount;
    }

    public void ChangeFrameDirection(Vector3 newDirection)
    {
        _currentFrameDirection = newDirection.normalized;
    }

    public void ChangeFramePosition(Vector3 newPosition)
    {
        _currentFramePosition = newPosition;
    }

    public void MoveInDirection()
    {
        ChangeFramePosition(_currentFramePosition + Mathf.Min(_currentFrameDistanceRemaining, MAX_STEP_LENGTH) * _currentFrameDirection);
        ReduceFrameDistanceRemaining(Mathf.Min(_currentFrameDistanceRemaining, MAX_STEP_LENGTH));
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

    public void Bounce(RaycastHit hitInfo)
    {
        Vector3 sphereCenter = hitInfo.point + hitInfo.normal * 0.5f;
        ChangeFramePosition(sphereCenter);
        ChangeFrameDirection(Vector3.Reflect(_currentFrameDirection, hitInfo.normal));
        ReduceFrameDistanceRemaining(hitInfo.distance);
        _realSpeed = _maxSpeed;
        _currentFrameCollisions.Add(sphereCenter);
        isSwitchingSide = false;
    }
    
    public void PassThrough(RaycastHit hitInfo)
    {
        ChangeFramePosition(hitInfo.point);
        ReduceFrameDistanceRemaining(hitInfo.distance);
    }

    void GetHit(Vector3 direction, float accSpeed = 1)
    {
        _direction = direction.normalized;
        _maxSpeed *= accSpeed;
        _realSpeed = _maxSpeed;
        isSwitchingSide = false;
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

    private void OnDrawGizmos()
    {
        // Spheres
        Gizmos.color = Color.red;
        // Collisions
        foreach (var VARIABLE in _currentFrameCollisions)
        {
            Gizmos.DrawWireSphere(VARIABLE, 0.5f);
        }
        // Final destination
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_currentFrameDestination, 0.5f);
        
        // Lines
        Gizmos.color = Color.yellow;
        Vector3 startLine = transform.position;
        // Movements with collision
        foreach (var VARIABLE in _currentFrameCollisions)
        {
            Gizmos.DrawLine(startLine, VARIABLE);
            startLine = VARIABLE;
        }
        // Final movement
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startLine, _currentFrameDestination);
    }
}
