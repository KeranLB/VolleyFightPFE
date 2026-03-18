using System;
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

    private bool _CanJump;
    private bool _isWalled;
    private bool _isGrounded;
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
        
        Move(_moveInput.action.ReadValue<Vector2>(), -9.81f);
        
        if (_moveInput.action.IsPressed())
        {
            
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
    void Move(Vector2 moveinput, float gravity)
    {
        gameObject.transform.position += new Vector3(moveinput.x * _MoveSpeed* Time.deltaTime,
                                                     gravity * Time.deltaTime,
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
}
