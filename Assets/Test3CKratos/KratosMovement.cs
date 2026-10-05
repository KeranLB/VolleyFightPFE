using System;
using UnityEngine;

public class KratosMovement : MonoBehaviour
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
    [SerializeField] private float _slowFallFactor;
    [SerializeField] private bool _justJumped;
    [SerializeField] private Transform _feetSpot;
    [SerializeField] private float _groundCheckRaycastLength;

    private Vector3 _currentVelocity;
    private Vector2 _currentHorizontalVelocity;
    private Vector2 _direction;
    private float _forceToApply;
    private bool _isGrounded;
    public bool isGrounded => _isGrounded;
    private bool _isSlowFalling;
    public bool isSlowFalling => _isSlowFalling;
    private bool _canDoubleJump;
    private Ball _ball;

    [Header("Override movement")]
    public bool isOverriden;
    public Vector3 overrideVelocity;
    public bool isFrozen;

    #region Delegates
    public event Action OnPlayerJump;
    public event Action OnPlayerDoubleJump;
    #endregion

    private void Awake()
    {
        _ball = FindFirstObjectByType<Ball>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Completely prevent moving (aiming...)
        if (isFrozen) return;
        
        // Make the character face the ball
        if (!isOverriden)
        {
            Vector3 faceTarget = _ball.transform.position;
            faceTarget.y = characterModel.transform.position.y;
            characterModel.LookAt(faceTarget);
        }
        
        if (isOverriden)
        {
            _currentVelocity = Quaternion.AngleAxis(characterModel.eulerAngles.y, Vector3.up) * overrideVelocity;
        }
        else
        {
            UpdateVelocityFromInputs();
        }
        
        if (_currentVelocity.magnitude == 0) return;

        // Move body according to velocity
        RaycastHit hitInfo;
        
        // If hitting ground or ceiling, slide
        if (
            Physics.Raycast(
                _rigidbody.position, Mathf.Sign(_currentVelocity.y)*Vector3.up, out hitInfo,
                Mathf.Abs(_currentVelocity.y) * Time.fixedDeltaTime + 1.0f, collisionLayers
            )
        )
        {
            _currentVelocity = Vector3.ProjectOnPlane(_currentVelocity, hitInfo.normal);
            // TODO : snap to the right height
        }
        // Check walls on side
        if (
            Physics.SphereCast(
                _rigidbody.position, 0.5f, _currentVelocity.normalized, out hitInfo,
                _currentVelocity.magnitude*Time.fixedDeltaTime, collisionLayers
            )
        )
        {
            _currentVelocity = Vector3.ProjectOnPlane(_currentVelocity, hitInfo.normal);
            // If cornered, reset horizontal velocity
            if (
                Physics.SphereCast(
                    _rigidbody.position, 0.5f, _currentVelocity.normalized, out hitInfo,
                    _currentVelocity.magnitude*Time.fixedDeltaTime, collisionLayers
                )
            )
            {
                _currentVelocity.x = 0f;
                _currentVelocity.z = 0f;
            }
        }
        _rigidbody.MovePosition(_rigidbody.position + _currentVelocity * Time.fixedDeltaTime);
    }

    private void UpdateVelocityFromInputs()
    {
        // Get Inputs and recombine them for current rotation
        var world_dir = _playerInput.rawDirection.x * characterModel.right + _playerInput.rawDirection.y * characterModel.forward;
        Vector2 tmp = new Vector2(world_dir.x, world_dir.z);

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

        // Jump and fall
        // On the ground
        if (_isGrounded)
        {
            _canDoubleJump = true;
            if(_playerInput.holdsJump || _playerInput.pressedJump)
            {
                OnPlayerJump?.Invoke();
                _currentVelocity.y = _jumpForce;
                _justJumped = true;
            }
            else
            {
                _currentVelocity.y = 0f;
            }
        }
        else // In air
        {
            // Double jump
            if (_playerInput.pressedJump && _canDoubleJump)
            {
                OnPlayerDoubleJump?.Invoke();
                _currentVelocity.y = _jumpForce;
                _canDoubleJump = false;
                _justJumped = false;
            }
            // Ascending
            if (_currentVelocity.y > 0)
            {
                _isSlowFalling = false;
                // Short jump if we release the jump button
                if (_justJumped && _playerInput.releasedJump)
                {
                    _currentVelocity.y /= 2;
                }
            }
            else // Falling
            {
                _justJumped = false;
                // Slow fall if we hold jump button
                _isSlowFalling = _playerInput.holdsJump;
            }
            var realMaxFallSpeed = isSlowFalling ? _maxFallSpeed * _slowFallFactor : _maxFallSpeed;
            _currentVelocity.y = Mathf.Max(realMaxFallSpeed, _currentVelocity.y + _gravity * Time.fixedDeltaTime);
        }

        // Velocity determines our current direction
        _direction = _currentHorizontalVelocity.normalized; // * _forwardDirection;
    }

    /// <summary>
    /// Checks wether the character's feet are on a ground surface.
    /// </summary>
    /// <returns>Returns true if the character is on the ground, else false.</returns>
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
