using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class CameraManager : MonoBehaviour
{
    #region Components
    [Header("Components :")]
    [SerializeField] private GameObject _player;
    private Camera _camera;
    private PlayerInput _PlayerInput;
    private Transform _playerTransform;
    private Ball _ball;
    #endregion

    #region Ball Speed
    [Header("Ball Speed :")]
    [SerializeField] private float _minBallSpeed;
    [SerializeField] private float _maxBallSpeed;
    #endregion

    #region Camera Distance
    [Header("Camera Distance :")]
    [SerializeField] private bool _isCameraDistanceActive;
    [SerializeField] private float _minCameraDistance;
    [SerializeField] private float _maxCameraDistance;
    #endregion

    #region Fov
    [Header("Fov :")]
    [SerializeField] private bool _isFovActive;
    [SerializeField] private float _minFov;
    [SerializeField] private float _maxFov;
    #endregion

    #region Camera Follow
    [Header("Camera Follow :")]

    [Header("Center Arena :")]
    [SerializeField] private bool _isLookingCenterArena;
    [SerializeField] private Vector3 _CenterArenaPosition;

    [Header("Pivot Camera :")]
    [SerializeField] private Transform _pivotCamera;
    [SerializeField] [Range(0f,1f)] private float FollowForceCharacter;
    [SerializeField] [Range(0f, 1f)] private float FollowForceBall;
    [SerializeField] [Range(0f, 1f)] private float FollowForceCenterArena;
    [SerializeField] private float DampingForceYRotation;

    [Header("Camera :")]
    [SerializeField] private float _minXAngle;
    [SerializeField] private float _maxXAngle;
    [SerializeField] private float DampingForceXRotation;
    #endregion

    #region Activation
    [Header("Switches :")]
    [SerializeField] private bool _FollowBallOnY;
    [SerializeField] private bool _FollowBallOnX;
    [SerializeField] private bool _PivotFollow;

    [Header("Corner Focus :")]
    [SerializeField] private bool _cornersFocus;
    [SerializeField] private Vector3 _rightCornerPosition;
    [SerializeField] private Vector3 _leftCornerPosition;
    [SerializeField] private float _leftCornerForce;
    [SerializeField] private float _rightCornerForce;
    [SerializeField] private float _playerCornerForce;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _PlayerInput = _player.GetComponent<PlayerInput>();
        _camera = GetComponent<Camera>();
        _playerTransform = _player.transform;
        _ball = FindFirstObjectByType<Ball>();
    }

    // Update is called once per frame
    void Update()
    {
        CameraFollow();

        float distancePercent = GetBallSpeed();

        if (_isCameraDistanceActive)
        {
            SetNewDistanceCameraToPlayer(distancePercent);
        }

        if (_isFovActive)
        {
            SetNewFov(distancePercent);
        }
    }


    /// <summary>
    /// Make the camera follow the ball around the pivot point.
    /// </summary>
    /// 
    private void CameraFollow()
    {
        if (_PivotFollow)
        {
            PivotPosition();
        }
        if (_FollowBallOnX)
        {
            SelfRotation();
        }
        if (_FollowBallOnY)
        {
            PivotRotation();
        }
    }

    private void SelfRotation()
    {
        Quaternion selfRotationBall = LookAtTarget(transform, _ball.transform, DampingForceXRotation);
        Quaternion selfRotationCharacter = LookAtTarget(transform, _playerTransform, DampingForceXRotation);

        Quaternion selfRotation = Quaternion.Lerp(selfRotationBall, selfRotationCharacter, 0.5f);
        Vector3 rotationX = new Vector3(selfRotation.eulerAngles.x, 0f, 0f);

        transform.localEulerAngles = rotationX;
    }

    private void ActualiseCenterPosition()
    {
        Vector3 target = new Vector3(0f, 0f, 0f);
        if (_playerTransform.position.x > 0f)
        {
            _CenterArenaPosition = new Vector3(20f, 0f, _CenterArenaPosition.z);
        }
        else
        {
            _CenterArenaPosition = new Vector3(-20f, 0f, _CenterArenaPosition.z);
        }

        if (_playerTransform.position.z > 0f)
        {
            _CenterArenaPosition = new Vector3(_CenterArenaPosition.x, 0f, 20f);
        }
        else
        {
            _CenterArenaPosition = new Vector3(_CenterArenaPosition.x, 0f, -20f);
        }
    }

    private void PivotPosition()
    {
        Vector3 newPosition;

        if (_cornersFocus)
        {
            newPosition = Interpolation3D(_playerTransform.position, _leftCornerPosition, _rightCornerPosition, _playerCornerForce, _leftCornerForce, _rightCornerForce);
        }
        //ActualiseCenterPosition();
        else
        {

            newPosition = Interpolation3D(_playerTransform.position, _ball.transform.position, _CenterArenaPosition, FollowForceCharacter, FollowForceBall, FollowForceCenterArena);
        }
        
        _pivotCamera.position = Vector3.Lerp(_pivotCamera.position, newPosition, Mathf.Lerp(0.5f, 5f, 1 - _ball.currentSpeedLevelIndex / 10f) * Time.deltaTime);
    }

    private void PivotRotation()
    {
        Quaternion pivotRotation;

        if (_isLookingCenterArena)
        {
            Quaternion pivotCharacterRotation = LookAtTarget(_pivotCamera, _playerTransform, DampingForceYRotation);
            Quaternion pivotBallRotation = LookAtTarget(_pivotCamera, _ball.transform, DampingForceYRotation);
            pivotRotation = Quaternion.Lerp(pivotBallRotation, pivotCharacterRotation, 0.5f);
        }

        else
        {
            _pivotCamera.localPosition = new Vector3(0f, 0f, 0f);
            pivotRotation = LookAtTarget(_pivotCamera, _ball.transform, DampingForceYRotation);
        }

        Vector3 rotationY = new Vector3(0f, pivotRotation.eulerAngles.y, 0f);
        _pivotCamera.eulerAngles = rotationY;
    }

    private Quaternion LookAtTarget(Transform from, Transform target, float dampingForce)
    {
        Vector3 relativePos = target.position - from.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation = Quaternion.Lerp(from.rotation, rotation, Mathf.Lerp(dampingForce/10, dampingForce, 1 - _ball.currentSpeedLevelIndex / 10f) * Time.deltaTime);
        return rotation;
    }


    /// <summary>
    ///  Returns the percent distance between the Camera and the Ball, from a minimum and a maximum value.
    /// </summary>
    /// <returns>Float value between 0 and 1.</returns>
    private float GetBallSpeed()
    {
        int speed = _ball.currentSpeedLevelIndex;

        return GetPercent(speed, _minBallSpeed, _maxBallSpeed);
    }


    /// <summary>
    /// Set the Fov Value from a parcent from a minimum and a maximum value.
    /// </summary>
    /// <param name="percent">Float between 0 and 1.</param>
    private void SetNewDistanceCameraToPlayer(float percent)
    {
        float value = GetValueFromPercent(percent, _minCameraDistance, _maxCameraDistance);
        transform.localPosition += new Vector3(0f, 0f, value);
    }

    /// <summary>
    /// Set the Fov Value from a parcent from a minimum and a maximum value.
    /// </summary>
    /// <param name="percent">Float between 0 and 1.</param>
    private void SetNewFov(float percent)
    {
        float value = GetValueFromPercent(percent, _minFov, _maxFov);
        _camera.fieldOfView = value;
    }


    /// <summary>
    /// Return the percent of a value between a minimum value and a maximum value.
    /// </summary>
    /// <param name="value">float Value between minimum value and maximum value. (if not, it will be Clamp)</param>
    /// <param name="min">Minimum Value.</param>
    /// <param name="max">Maximum Value</param>
    /// <returns>Float Value between 0 and 1.</returns>
    private float GetPercent(float value, float min, float max)
    {
        float ecart = max - min;

        value = Mathf.Clamp(value,min,max);

        float percent = (value-min)/ecart;

        return percent;
    }


    /// <summary>
    /// Return a value between a minimum value and a maximum value from a percent.
    /// </summary>
    /// <param name="percent">F²loat between 0 and 1. (If not it will be Clamp)</param>
    /// <param name="min">Minimum Value.</param>
    /// <param name="max">Maximum Value</param>
    /// <returns>Float value between min and max</returns>
    private float GetValueFromPercent(float percent, float min, float max)
    {
        float ecart = max - min;

        percent = Mathf.Clamp01(percent);

        float value = (percent * ecart) + min;

        return value;
    }


    private Vector3 Interpolation3D(Vector3 vectorA, Vector3 vectorB, Vector3 vectorC, float forceA, float forceB, float forceC)
    {
        float resultX = vectorA.x * forceA + vectorB.x * forceB + vectorC.x * forceC;
        float resultY = vectorA.y * forceA + vectorB.y * forceB + vectorC.y * forceC;
        float resultZ = vectorA.z * forceA + vectorB.z * forceB + vectorC.z * forceC;
        Vector3 result = new Vector3(resultX,resultY,resultZ);
        return result;
    }
}
