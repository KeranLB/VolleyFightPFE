using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerCharacter : MonoBehaviour
{
    #region Variables

    #region Inputs

    //[SerializeField] private InputActionAsset _inputActionAsset;
    [FormerlySerializedAs("_moveInput")]
    [Header("Inputs :")]
    [SerializeField] private InputActionReference _moveInputAction;
    private Vector2 _moveInput;
    private Vector2 _lastDirection = new Vector2(0f,0f);
    [SerializeField] private InputActionReference _jumpInput;
    [SerializeField] private InputActionReference _attackInput;
    [SerializeField] private InputActionReference _receptionInput;
    private float _holdJumpInput = 0f;

    #endregion

    #region Movement

    [Header("Movement :")]

    private Vector3 _velocity;

    [Header("Speed :")] 
    [SerializeField] private bool TestTrue;

    private int _frameCurve;
    [SerializeField] private List<float> _speeds;
    private int _indexSpeed = 0;
    [SerializeField] private float _maxMoveSpeed;
    [SerializeField] private float _moveSpeed;
    
    [Header("Acceleration Curve :")]
    [SerializeField] private AnimationCurve _accelerationCurve;
    [SerializeField] private float _accelerationTimeDuration;
    [SerializeField] private float _accelerationCurrentTime = 0f;
    
    [Header("Inertia Curve :")]
    [SerializeField] private AnimationCurve _inertiaCurve;
    [SerializeField] private float _inertiaTimeDuration;
    [Header("")] // Spacer
    #endregion
    
    #region Component
    [Header("Component :")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private Transform _meshCharacter;
    [SerializeField] private float _gravityForce;



    #endregion

    #region Booleans

    private bool _isMoving = false;
    private bool _canJump = true;
    private bool _isWalled;
    private bool _isGrounded = false;
    private bool _isJumping;

    #endregion

    #region Health
    
    [SerializeField] int _maxHealth;
    private int _currentHealth;
    private bool _isDead = false;
    private Vector3 _deathPosition;
    
    #endregion
    
    #region Stats
    [Header("Stats :")]

    [SerializeField] private float _jumpForce;
    [SerializeField] float _doubleJumpForce;

    #endregion

    #endregion

    #region MainFunctions

    void Start()
    {
        TestTrue = true;
       _currentHealth = _maxHealth;

       _accelerationCurrentTime = 0f;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        if (!_isDead)
        {
            Move();
        }
    }

    // Update is called once per frame
    void Update()
    {

        _moveInput = _moveInputAction.action.ReadValue<Vector2>();
        _isJumping = _jumpInput.action.WasPressedThisFrame();
        if (_isJumping && CheckShouldJump())
        {
            _velocity.y = _jumpForce;
        }

        if (Input.GetKeyDown("e"))
        {
            GetDamage(10);
        }

        if (Input.GetKeyDown("r") && _isDead)
        { 
            RespawnCharacter();
        }
    }

    #endregion

    #region MovementCharacter

    void SimGravity()
    {
        if (!_isGrounded)
        {
            _velocity.y += _gravityForce * Time.fixedDeltaTime;
        }
        else
        {
            _velocity.y = 0f;
        }
    }

    void Move()
    {

        Vector3 normDir = _moveInput.normalized;
        _velocity.x = normDir.x * _maxMoveSpeed;
        _velocity.z = normDir.y * _maxMoveSpeed;

        if (_isGrounded)
        {
            _velocity.y = Mathf.Max(_velocity.y, 0f);
        }
        else
        {
            _velocity.y += _gravityForce * Time.fixedDeltaTime;
        }

        //RotateCharacter();
        CharacterRotation();
        
        int layer = LayerMask.GetMask("Ground");
        if (Physics.Raycast(_rb.position, _velocity.normalized, out var hit, _velocity.magnitude * Time.fixedDeltaTime, layer))
        {
            _rb.MovePosition(hit.point - _velocity.normalized);
            //_rb.MovePosition(hit.point + Vector3.up * 4f);
        }
        else
        {
            _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
        }
    }

    void CharacterRotation()
    {
        _meshCharacter.forward = new Vector3(_moveInput.x, 0f, _moveInput.y);
    }


    private void AccelerationTest()
    {
        _accelerationCurrentTime++;
        _accelerationCurrentTime = Mathf.Clamp(_accelerationCurrentTime,0f,_accelerationTimeDuration);
        _moveSpeed = _maxMoveSpeed *  _accelerationCurve.Evaluate(_accelerationCurrentTime / _accelerationTimeDuration);
    }

    private void InertiaTest()
    {
        _accelerationCurrentTime--;
        _accelerationCurrentTime = Mathf.Clamp(_accelerationCurrentTime, 0f, _inertiaTimeDuration);
        _moveSpeed = _maxMoveSpeed * _inertiaCurve.Evaluate(_accelerationCurrentTime / _inertiaTimeDuration);
    }

    #endregion

    #region Attack



    void LaunchAttack()
    {
        
    }

    void GroundAttack()
    {

    }

    void AerialAttack()
    {
        
    }

    void SmashAttack()
    {
        
    }

    #endregion

    #region Jump



    void Jump()
    {
        _velocity.y = _jumpForce;
        //gameObject.transform.position += new Vector3(0f, _jumpForce * Time.deltaTime, 0f);
    }

    IEnumerator FirstJumpDuration()
    {
        yield return new WaitForSeconds(0.5f);
        _isJumping = false;
    }

    void DoubleJump()
    {
        
    }

    void WallJump()
    {
        
    }
    
    #endregion
    
    #region Reception



    void LaunchReception()
    {
        
    }

    void PassToSelf()
    {
        
    }

    void PassToAlly()
    {
        
    }

    void Slide()
    {
        
    }

    void Recover()
    {
        
    }

    #endregion

    #region Checker

    void GroundCheck()
    {
        RaycastHit hit;
        Vector3 startPosition = _rb.position + new Vector3(0f, -3f, 0f);
        Vector3 direction = Vector3.down;
        float duration = 1f;
        int layer = LayerMask.GetMask("Ground");

        Debug.DrawRay(startPosition, direction, Color.red, duration);

        _isGrounded = Physics.Raycast(_groundCheck.position, direction, out hit, 0.3f, layer);
        if (_isGrounded)
        {
            _canJump = true;
        }
    }

    void WallCheck()
    {
        RaycastHit hit;
        Vector3 direction = _wallCheck.forward;
        float duration = 1f;
        int layer = LayerMask.GetMask("Ground");

        Debug.DrawRay(_wallCheck.position, direction, Color.yellow, duration);

        _isWalled = Physics.Raycast(_wallCheck.position, direction, out hit, 0.3f, layer);
        if (_isWalled)
        {
            Debug.Log("Is Walled");
            _canJump = true;
        }
    }

    bool CheckShouldJump()
    {
        if (_canJump && _isGrounded)
        {
            return true;
        }
        if (_canJump && _isWalled)
        {
            return true;
        }
        else if (_canJump)
        {
            _canJump = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckShouldReception()
    {

    }

    void CheckShouldAttack()
    {

    }

    #endregion

    #region LifeSystem

    public void GetDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
            _deathPosition = _rb.position;
            _rb.position = new Vector3(1000f, 1000f, 1000f);
        }
        print("You have " + _currentHealth + "Hp remaining !");
    }

    public void RespawnCharacter()
    {
        _rb.position = _deathPosition;
        _currentHealth = _maxHealth;
        _isDead = false;
    }

    #endregion
}
