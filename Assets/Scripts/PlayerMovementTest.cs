using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementTest : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveInputAction;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _actualAcceleration;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _maxMoveSpeed;
    private float _maxInertia = 5f;
    private float _actualInertia;
    [SerializeField] private float _inertia = 1f;

    
    private Vector3 _moveDirection;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveSpeed = 0f;
        _actualAcceleration = 0f;
    }

    void FixedUpdate()
    {
        if (_moveInputAction.action.IsPressed())
        {
            Move();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_moveInputAction.action.IsPressed())
        {
            InputMovement();
        }
        else if (_moveInputAction.action.WasReleasedThisFrame())
        {
            StartCoroutine(Inertia());
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
        _actualAcceleration += _acceleration *  Time.deltaTime;
        _moveSpeed = _moveSpeed + _actualAcceleration * Time.deltaTime;
        if (_moveSpeed >= _maxMoveSpeed)
        {
            _moveSpeed = _maxMoveSpeed;
        }
        _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.deltaTime);
    }

    void InertiaTest()
    {
        _actualAcceleration -= _acceleration * Time.deltaTime;
        if (_actualAcceleration  <= 0f)
        {
            _actualAcceleration = 0f;
        }
        print( _actualAcceleration);
        _moveSpeed -=  _actualAcceleration * Time.deltaTime;
        if (_moveSpeed <= 0f)
        {
            _moveSpeed = 0f;
        }
        _rb.MovePosition(_rb.position + _moveDirection * _moveSpeed * Time.deltaTime);
    }

    IEnumerator Inertia()
    {
        print("rentre dans la coroutine");
        _actualInertia = 0f;
        while (_moveSpeed >= 0f)
        {
            _actualInertia += _inertia * Time.deltaTime;
            _moveSpeed -= _actualInertia * Time.deltaTime;
            _rb.MovePosition(_rb.position + _moveDirection* _moveSpeed * Time.deltaTime);
            print("effectue un Tick");
            yield return null;
        }
        print("fin de la coroutine");
    }
}
