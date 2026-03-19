using System;
using System.Collections;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    #region Inputs
    
    //[SerializeField] private InputActionAsset _inputActionAsset;
    [Header("Inputs :")]
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private InputActionReference _jumpInput;
    [SerializeField] private InputActionReference _attackInput;
    [SerializeField] private InputActionReference _receptionInput;
    private float _holdJumpInput = 0f;
    
    #endregion

    #region Booleans

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
    [SerializeField] float _MoveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] float _doubleJumpForce;
    
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       _currentHealth = _maxHealth;
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
        Move(_moveInput.action.ReadValue<Vector2>());


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
    void Move(Vector2 moveinput)
    {
        gameObject.transform.position += new Vector3(moveinput.x * _MoveSpeed* Time.deltaTime,
                                                     0f,
                                                     moveinput.y * _MoveSpeed * Time.deltaTime);
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
