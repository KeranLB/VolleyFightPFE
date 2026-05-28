using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Ball : MonoBehaviour
{
    #region Speeds

    [SerializeField] private float _maxSpeed;
    private float _slowSpeed = 0f;
    private float _realSpeed;
    public float baseSpeed = 10.0f;
    public float baseDamage = 1f;
    [SerializeField] private float _passSpeed;
    [SerializeField] private List<SO_BallSpeedLevel> _speedLevels;
    public SO_BallSpeedLevel currentSpeedLevel;
    private Coroutine _decelerationCoroutine;
    private Coroutine _downGradeCoroutine;
    public int currentSpeedLevelIndex;

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
    public bool isSwitchingSide;
    
    #endregion
    
    #region Subcomponents
        private Rigidbody _rigidbody;
        private MeshRenderer _meshRenderer;
        private Animator _animator;
        private LineRenderer _lineRenderer;
    #endregion

    #region Physics

    private float _radius;
    private Vector3 _currentFramePosition;
    private Vector3 _currentFrameDirection;
    private float _currentFrameDistanceRemaining;
    private const float MAX_STEP_LENGTH = 0.5f;
    private const int MAX_STEP_NB = 100;
    public bool isFreezeFrame = false;
    
    #endregion
    
    #region DEBUG
    
    [SerializeField]
    private List<Vector3> _currentFrameCollisions;
    private Vector3 _currentFrameDestination;

    #endregion
    
    #region Feedbacks

    private VisualEffect _vfxImpact;
    
    #endregion

    #region Delegates

    public static event Action<int> OnSpeedLevelChanged;
    public event Action OnBallBounce;
    public event Action<Player> OnBallPlayer;
    public event Action<Player> OnBallHit;

    #endregion


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _vfxImpact = GetComponentInChildren<VisualEffect>();
        _animator = GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentFramePosition = _rigidbody.position;
        _radius = transform.localScale.x / 2;
        _lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ChangeSpeedLevel(currentSpeedLevelIndex + 1);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            ChangeSpeedLevel(currentSpeedLevelIndex - 1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
           GetHit(Vector3.back);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            GetHit(Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            GetHit(Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            GetHit(Vector3.forward);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            GetHit(Vector3.down);
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            GetHit(Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            GetHit(Vector3.up*2 + Vector3.forward + Vector3.left);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            GetHit(Vector3.up + Vector3.forward*3 + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            GetHit(Vector3.down + Vector3.forward + Vector3.left*2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            GetHit(Vector3.down*2.5f + Vector3.forward + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            ChangeTeam(teamPossess==Teams.TeamA ? Teams.TeamB : Teams.TeamA);
        }
    }

    private void FixedUpdate()
    {
        if (isFreezeFrame) return;
        
        _rigidbody.MovePosition(_currentFramePosition);
        _currentFrameCollisions.Clear();
        _currentFrameDirection = _direction.normalized;
        _currentFrameDistanceRemaining = _realSpeed * Time.fixedDeltaTime;
        
        // Check we're not already overlapping a collider that would be ignored by SphereCast
        Collider[] cols = Physics.OverlapSphere(_currentFramePosition, _radius);
        foreach (Collider collider in cols)
        {
            if(collider.TryGetComponent(out HitZone hitZone))
            {
                hitZone.OnOverlap(this);
            }
        }
        
        // Simulate next position until all collisions are resolved
        int hardLimit = MAX_STEP_NB; // Don't get stuck in infinite loop
        while (_currentFrameDistanceRemaining > 0 && hardLimit > 0)
        {
            CheckCollisionAhead();
            hardLimit--;
        }

        if (hardLimit == 0)
        {
            Debug.LogError("HARD LIMIT REACHED");
        }
        _direction = _currentFrameDirection;
        _currentFrameDestination = _currentFramePosition;
    }

    public void CheckCollisionAhead()
    {
        if(
            Physics.SphereCast(_currentFramePosition, _radius, _currentFrameDirection, out RaycastHit hit, Mathf.Min(_currentFrameDistanceRemaining, MAX_STEP_LENGTH))
            //Physics.Raycast(_currentFramePosition, _currentFrameDirection, out RaycastHit hit, _currentFrameDistanceRemaining)
            && hit.collider.TryGetComponent<HitZone>(out HitZone zone)
        )
        {
            zone.OnTrajectory(this, hit);
        }
        else
        {
            MoveInDirection();
        }
    }

    public void ReduceFrameDistanceRemaining(float amount)
    {
        _currentFrameDistanceRemaining -= amount;
    }

    public void ChangeFrameDirection(Vector3 newDirection)
    {
        _currentFrameDirection = newDirection.normalized;
    }

    public void ChangeFramePosition(Vector3 newPosition)
    {
        _currentFramePosition = newPosition;
    }

    public void MoveInDirection()
    {
        ChangeFramePosition(_currentFramePosition + Mathf.Min(_currentFrameDistanceRemaining, MAX_STEP_LENGTH) * _currentFrameDirection);
        ReduceFrameDistanceRemaining(Mathf.Min(_currentFrameDistanceRemaining, MAX_STEP_LENGTH));
    }

    public void ChangeTeam(Teams team)
    {
        teamPossess = team;
        _meshRenderer.material.color = team switch
        {
            Teams.TeamA => Color.blue,
            Teams.TeamB => Color.yellow,
            _ => Color.gray
        };
    }

    public void ChangeSpeedLevel(int i)
    {
        currentSpeedLevelIndex = Mathf.Clamp(i, 0, _speedLevels.Count - 1);
        currentSpeedLevel = _speedLevels[currentSpeedLevelIndex];
        _maxSpeed = GetFinalSpeed();
        _slowSpeed = GetSlowSpeed();
        OnSpeedLevelChanged?.Invoke(currentSpeedLevelIndex);
    }

    public void Bounce(RaycastHit hitInfo)
    {
        Bounce(hitInfo.point, hitInfo.normal, hitInfo.distance);
    }

    public void Bounce(Vector3 position, Vector3 normal, float distance)
    {
        OnBallBounce?.Invoke();
        Vector3 sphereCenter = position + normal * _radius;
        ChangeFramePosition(sphereCenter);
        ChangeFrameDirection(Vector3.Reflect(_currentFrameDirection, normal));
        ReduceFrameDistanceRemaining(distance);
        _currentFrameCollisions.Add(sphereCenter);
        isSwitchingSide = false;
        VFXEventAttribute attribute = new VFXEventAttribute(_vfxImpact.CreateVFXEventAttribute());
        attribute.SetVector3("position", _currentFramePosition);
        _vfxImpact.SendEvent("Bounce", attribute);
    }

    public void Bunt(Player player, Vector3 position)
    {
        OnBallPlayer?.Invoke(player);
        StopSimulation(position);
        GetHit(Vector3.up);
        _currentFrameCollisions.Add(position);
    }
    
    public void PassThrough(RaycastHit hitInfo)
    {
        ChangeFramePosition(hitInfo.point);
        ReduceFrameDistanceRemaining(hitInfo.distance);
    }

    public void GetHit(Vector3 direction, int speedLevelChange = 0, Player player = null)
    {
        OnBallHit?.Invoke(player);
        ChangeFrameDirection(direction);
        _direction = direction.normalized;
        ChangeSpeedLevel(currentSpeedLevelIndex + speedLevelChange);
        _realSpeed = _maxSpeed;
        //StartCoroutine(DecelerationBall());
    }

    private IEnumerator DecelerationBall()
    {
        float time = 0f;
        float playBackSpeed = 1 / currentSpeedLevel.decelerationDuration;
        while (time <= 1)
        {
            time += playBackSpeed * Time.fixedDeltaTime;
            float tmp = currentSpeedLevel.decelerationCurve.Evaluate(time);
            _realSpeed = Mathf.Lerp(_slowSpeed, _maxSpeed, tmp);
            yield return new WaitForFixedUpdate();
        }
        _decelerationCoroutine = null;
        _downGradeCoroutine = StartCoroutine(DownGradeLevel());
    }

    private IEnumerator DownGradeLevel()
    {
        if (currentSpeedLevelIndex <= 1) yield break;
        
        yield return new WaitForSeconds(currentSpeedLevel.speedLevelDuration);
        ChangeSpeedLevel(currentSpeedLevelIndex - 1);
        _realSpeed = _slowSpeed;
        if (currentSpeedLevelIndex >= 2)
        {
            _downGradeCoroutine = StartCoroutine(DownGradeLevel());
        }
        else
        {
            _downGradeCoroutine = null;
        }
    }

    public void Freeze()
    {
        isFreezeFrame = true;
        if (_downGradeCoroutine != null)
        {
            StopCoroutine(_downGradeCoroutine);
            _downGradeCoroutine = null;
        }
        if (_decelerationCoroutine != null)
        {
            StopCoroutine(_decelerationCoroutine);
            _decelerationCoroutine = null;
        }
    }

    public void UnFreeze()
    {
        isFreezeFrame = false;
        _decelerationCoroutine = StartCoroutine(DecelerationBall());
    }

    public void ShowLine()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.material.SetVector("_Center", _currentFramePosition);
        if (Physics.Raycast(_currentFramePosition, _direction, out RaycastHit hit, Mathf.Infinity))
        {
            _lineRenderer.SetPosition(0, _currentFramePosition);
            _lineRenderer.SetPosition(1, hit.point);
        }
    }

    public void HideLine()
    {
        _lineRenderer.enabled = false;
    }

    public void StopSimulation(Vector3 spot)
    {
        _currentFramePosition = spot;
        _currentFrameDistanceRemaining = 0f;
    }

    public float GetFinalDamage()
    {
        return baseDamage * currentSpeedLevel.damageMultiplier;
    }

    public float GetFinalSpeed()
    {
        return baseSpeed * currentSpeedLevel.speedMultiplier;
    }

    public float GetSlowSpeed()
    {
        return baseSpeed * currentSpeedLevel.slowSpeedMultiplier;
    }
    
    void GetBlocked(Vector3 target)
    {
        _target = target;
        _realSpeed = _passSpeed;
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

    private void OnDrawGizmos()
    {
        // Spheres
        Gizmos.color = Color.red;
        // Collisions
        foreach (var VARIABLE in _currentFrameCollisions)
        {
            Gizmos.DrawWireSphere(VARIABLE, 0.5f);
        }
        // Final destination
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_currentFrameDestination, 0.5f);
        
        // Lines
        Gizmos.color = Color.yellow;
        Vector3 startLine = transform.position;
        // Movements with collision
        foreach (var VARIABLE in _currentFrameCollisions)
        {
            Gizmos.DrawLine(startLine, VARIABLE);
            startLine = VARIABLE;
        }
        // Final movement
        Gizmos.color = Color.green;
        Gizmos.DrawLine(startLine, _currentFrameDestination);
    }
}
