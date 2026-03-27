using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    public LayerMask collisionLayers;
    public Transform characterModel;

    [Header("Horizontal Movement")]
    [SerializeField] private float _groundAcceleration;
    [SerializeField] private float _groundFriction;
    [SerializeField] private float _maxHorizontalSpeed;
    
    [Header("Vertical Movement")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravity;
    [SerializeField] private float _maxFallSpeed;
    [SerializeField] private Transform _feetSpot;
    [SerializeField] private float _groundCheckRaycastLength;
    
    private Vector3 _currentVelocity;
    private Vector2 _currentHorizontalVelocity;
    private Vector2 _direction;
    private float _forceToApply;
    private bool _isGrounded;
    private bool _canDoubleJump;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var tmp = _playerInput.currentDirection;
        if (tmp.magnitude > 0)
        {
            characterModel.forward = Vector3.right * tmp.x + Vector3.forward * tmp.y;
        }
    }

    private void FixedUpdate()
    {
        // Get Inputs
        var tmp = _playerInput.currentDirection.normalized;

        // Get GroundCheck
        _isGrounded = GroundCheck();
        
        // What velocity do we want to reach
        Vector2 targetVelocity;
        if(tmp.magnitude>0)
        {
            // With movement input : full speed at direction
            targetVelocity = tmp * _maxHorizontalSpeed;
        }
        else
        {
            // Without movement input : stop
            targetVelocity = Vector2.zero;
        }

        // What will the acceleration be
        if (targetVelocity.magnitude == 0)
        {
            // If we want to stop : ground friction
            _forceToApply = _groundFriction;
        }
        else if(Vector2.Angle(_direction, targetVelocity.normalized) < 90)
        {
            // If we continue to move : acceleration
            _forceToApply = _groundAcceleration;
        }
        else
        {
            // We move opposite to our direction : acceleration AND friction
            _forceToApply = _groundFriction + _groundAcceleration;
        }
        
        // Apply forces to the velocity
        _currentHorizontalVelocity = Vector2.MoveTowards(_currentHorizontalVelocity, targetVelocity, _forceToApply * Time.fixedDeltaTime);
        
        _currentVelocity.x = _currentHorizontalVelocity.x;
        _currentVelocity.z = _currentHorizontalVelocity.y;

        if (_isGrounded)
        {
            _canDoubleJump = true;
            if(_playerInput.holdsJump || _playerInput.pressedDoubleJump)
            {
                _currentVelocity.y = _jumpForce;
            }
            else
            {
                _currentVelocity.y = 0f;
            }
        }
        else
        {
            if (_playerInput.pressedDoubleJump && _canDoubleJump)
            {
                _currentVelocity.y = _jumpForce;
                _canDoubleJump = false;
            }
            _currentVelocity.y = Mathf.Max(_maxFallSpeed, _currentVelocity.y + _gravity * Time.fixedDeltaTime);
        }
        
        // Velocity determines our current direction
        _direction = _currentHorizontalVelocity.normalized;
        if (_currentVelocity.magnitude == 0) return;

        // Move body according to velocity
        RaycastHit hitInfo;
        
        // If hitting ground or ceiling, snap and reset vertical speed
        if (
            Physics.Raycast(
                _rigidbody.position, Mathf.Sign(_currentVelocity.y)*Vector3.up, out hitInfo,
                Mathf.Abs(_currentVelocity.y) * Time.fixedDeltaTime + 0.5f, collisionLayers
            )
        )
        {
            _rigidbody.MovePosition(hitInfo.point+hitInfo.normal);
            _currentVelocity.y = 0f;
            return;
        }
        // Check walls on side
        if (
            Physics.Raycast(
                _rigidbody.position, _currentVelocity.normalized, out hitInfo,
                _currentVelocity.magnitude * Time.fixedDeltaTime + 0.5f, collisionLayers
            )
        )
        {
            _currentVelocity = Vector3.ProjectOnPlane(_currentVelocity, hitInfo.normal);
            // If cornered, reset horizontal velocity
            if (
                Physics.Raycast(
                    _rigidbody.position, _currentVelocity.normalized, out hitInfo,
                    _currentVelocity.magnitude * Time.fixedDeltaTime + 0.5f, collisionLayers
                )
            )
            {
                _currentVelocity.x = 0f;
                _currentVelocity.z = 0f;
            }
        }
        _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * Time.fixedDeltaTime);
    }

    private bool GroundCheck()
    {
        return Physics.Raycast(_feetSpot.position, Vector3.down, _groundCheckRaycastLength, collisionLayers);
    }

    public void FullStop()
    {
        _currentVelocity = Vector3.zero;
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        GUI.Label(new Rect(10, 30, 200, 20), "Velocity: "+ _currentVelocity, style);
        GUI.Label(new Rect(10, 50, 200, 20), "Acceleration: "+ _forceToApply, style);
    }
}
