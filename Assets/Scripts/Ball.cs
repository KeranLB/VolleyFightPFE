using System.Collections;
using UnityEngine;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PhysicSim()
    {
        
    }

    void Bounce()
    {
        
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
