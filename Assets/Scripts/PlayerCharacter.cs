using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class PlayerCharacter : MonoBehaviour
{
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



    #endregion

    #region Booleans

    private bool _isMoving = false;
    private bool _CanJump = true;
    private bool _isWalled;
    private bool _isGrounded = false;
    private bool _isJumping;

    #endregion

    #region Health
    
    [SerializeField] int _maxHealth;
    private int _currentHealth;
    
    #endregion
    
    #region Stats
    [Header("Stats :")]

    [SerializeField] private float _jumpForce;
    [SerializeField] float _doubleJumpForce;
    
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _currentHealth = _maxHealth;

       _accelerationCurrentTime = 0f;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        _moveInput = _moveInputAction.action.ReadValue<Vector2>();
        if (_moveInputAction.action.IsPressed())
        {
            //AccelerationTest();
            if (_indexSpeed < _speeds.Count-1)
            {
                _indexSpeed++;
            }
            AccelerationTest();
            Move();
        }
        else if (_indexSpeed > 0 && TestTrue)
        {
            _indexSpeed--;
            Move();
        }
        else if (_accelerationCurrentTime <= 0f && TestTrue!)
        {
            InertiaTest();
            Move();
        }
        /*
        else if (_currentAccelerationTime > 0)
        {
            print("last direction = " + _lastDirection);
            DecelerationTest();
            Move();
        }

        
        if (_moveInput.action.WasPressedThisFrame())
        {
            _isMoving = true;
            StartCoroutine(AccelerationMove());
        }
        else if (_moveInput.action.WasReleasedThisFrame())
        {
            _isMoving = false;
            StopCoroutine(AccelerationMove());
            StartCoroutine(DecelerationMove());
        }

        if (_isMoving)
        {
            Move(_moveInput.action.ReadValue<Vector2>());
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (_jumpInput.action.WasPressedThisFrame())
        {
            _isJumping = true;
            StartCoroutine(FirstJumpDuration());
        }
        else if (_jumpInput.action.WasReleasedThisFrame())
        {
            _isJumping = false;
            StopCoroutine(FirstJumpDuration());
        }

        if (_isJumping)
        {
            Jump();
        }
    }

    void GroundCheck()
    {
        RaycastHit hit;
        _isGrounded = Physics.Raycast(transform.position,
            Vector3.down,
            out hit,
            1f,
            LayerMask.GetMask("Ground"));
    }
    void Move()
    {
        Vector3 newPosition;
        if (TestTrue)
        {
            newPosition = _rb.position + new Vector3(_moveInput.x * _speeds[_indexSpeed], 0f,_moveInput.y * _speeds[_indexSpeed]);
        }
        else
        { 
            newPosition = _rb.position + new Vector3(_moveInput.x * _moveSpeed, 0f, _moveInput.y * _moveSpeed);
        }
        _rb.MovePosition(newPosition);
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

    #region Attack

    void CheckShouldAttack()
    {
        
    }

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

    void CheckShouldJump()
    {
        
    }

    void Jump()
    {
        gameObject.transform.position += new Vector3(0f, _jumpForce * Time.deltaTime, 0f);
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

    void CheckShouldReception()
    {
        
    }

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

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.layer.ToString());
        if (other.gameObject.layer == 3)
        {
            print("IsGrounded");
            _isGrounded = true;
            _CanJump = true;
        }
    }

}
