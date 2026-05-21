using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovementTest : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveInputAction;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _actualAcceleration;
    private float _acceleration;
    [SerializeField] private float _maxMoveSpeed;
    private float _maxInertia = 5f;
    private float _actualInertia;
    [SerializeField] private float _inertia = 1f;

    [SerializeField] private float _accelerationTime;
    [SerializeField] private float _friction;

    private Vector3 _vAcceleration;
    private Vector3 _vActualAcceleration;
    private Vector3 _v;
    
    private Vector3 _moveDirection;

    #region Gravity
    [SerializeField] private Transform GroundChecker;
    [SerializeField] private Vector3 _scaleGroundChecker;
    [SerializeField] private SphereCollider _sphereTest;

    private bool _isGrounded;
    private Vector3 Velocity;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        _acceleration = _maxMoveSpeed / _accelerationTime;
        _friction = _maxMoveSpeed * 2;

        _moveSpeed = 0f;
        _actualAcceleration = 0f;
    }

    void FixedUpdate()
    {
        if (_moveInputAction.action.IsPressed())
        {
            Acceleration();
        }
        else
        {
            Friction();
        }
        Move();
        Gravity();
    }

    // Update is called once per frame
    void Update()
    {
        if (_moveInputAction.action.IsPressed())
        {
            InputMovement();
        }
    }

    void InputMovement()
    {
        float InputX = _moveInputAction.action.ReadValue<Vector2>().x;
        float InputY = _moveInputAction.action.ReadValue<Vector2>().y;
        _moveDirection = new Vector3(InputX, 0f, InputY);
    }

    void Move()
    {
        _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.deltaTime);
    }

    void Acceleration()
    {
        float result = _actualAcceleration + _acceleration * Time.deltaTime;
        _actualAcceleration = Mathf.Clamp(result , 0f, _maxMoveSpeed);

        Vector3 acceTest = new Vector3(result, result, result); 
        Vector3 minVector = new Vector3(0f, 0f, 0f);
        Vector3 maxVector = new Vector3(0f, 0f,0f) * _maxMoveSpeed;
        Vector3.ClampMagnitude(acceTest, _maxMoveSpeed);


        result = _moveSpeed + _actualAcceleration * Time.deltaTime;
        _moveSpeed = Mathf.Clamp(result ,0f, _maxMoveSpeed);
    }

    void Friction()
    {
        float result = _moveSpeed - _friction * Time.deltaTime;
        _moveSpeed = Mathf.Clamp(result, 0f, _maxMoveSpeed);
    }

    void Gravity()
    {
        if (!GroundCheck())
        {
            Vector3 result = _rb.position + new Vector3(0f, -10f, 0f) * Time.fixedDeltaTime;
            //result.y = Mathf.Clamp(result.y, -9.81f, 0f);

            _rb.MovePosition(result);
        }
        else if (GroundCheck())
        {
            
        }
    }

    bool GroundCheck()
    {
        Quaternion orientation = Quaternion.identity;
        LayerMask layer = LayerMask.GetMask("Ground");
        Array box = Physics.OverlapBox(GroundChecker.position, _scaleGroundChecker, orientation, layer);
        bool Condition = Physics.CheckBox(GroundChecker.position, _scaleGroundChecker, orientation, layer);

        bool Condition2 = Physics.CheckSphere(_sphereTest.transform.position, _sphereTest.radius, layer, QueryTriggerInteraction.Collide);
        Collider[] collider = Physics.OverlapSphere(_sphereTest.transform.position, _sphereTest.radius, layer);

        //Vector3 collisionPosition = collider[1].

        Vector3 endline = new Vector3(GroundChecker.position.x, GroundChecker.position.y + 10f * Time.deltaTime , GroundChecker.position.z);
        Debug.DrawLine(GroundChecker.position, endline, Color.blue, 1f);
        
        if (Condition2)
        {
            print("CheckBox");
            return Condition;
        }
        else if (Physics.Raycast(GroundChecker.position, Vector3.down, 0.1f, layer))
        {
            print("Raycast");
            return true;
        }
        else
        {
            return false;
        }
    }
}
