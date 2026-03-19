using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _realSpeed += 5;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            _realSpeed -= 5;
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
           _direction = Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            _direction = Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            _direction = Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            _direction = Vector3.forward;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            _direction = Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            _direction = Vector3.up;
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            _direction = Vector3.up + Vector3.forward + Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            _direction = Vector3.up + Vector3.forward + Vector3.right;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _direction = Vector3.down + Vector3.forward + Vector3.left;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            _direction = Vector3.down + Vector3.forward + Vector3.right;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(transform.position + _realSpeed * Time.fixedDeltaTime * _direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

    void PhysicSim()
    {
        
    }

    public void Bounce(Vector3 normal)
    {
        Debug.Log("Bounce at " + normal);
        _direction = Vector3.Reflect(_direction, normal);
    }

    void GetHit(Vector3 direction, float accSpeed)
    {
        _direction = direction;
    }

    void GetRecpt(Vector3 target)
    {
        _target = target;
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
