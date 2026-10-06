using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

    #region Ball Distance
    [Header("Ball Distance :")]
    [SerializeField] private float _minBallDistance;
    [SerializeField] private float _maxBallDistance;
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
    [SerializeField] private Transform _pivotCamera;
    [SerializeField] private float _minXAngle;
    [SerializeField] private float _maxXAngle;
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

        float distancePercent = GetDistancePercentCameraToBall();

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
    private void CameraFollow()
    {
        Vector3 relativePos = _ball.transform.position - _player.transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);

        rotation = Quaternion.Lerp(_pivotCamera.rotation, rotation, Mathf.Lerp(0.5f, 5f, 1 - _ball.currentSpeedLevelIndex / 10f) * Time.deltaTime);
        Debug.Log(rotation.x);
        //rotation.x = Mathf.Clamp(rotation.eulerAngles.x, _minXAngle, _maxXAngle);
        Vector3 tmpRotation = new Vector3(0f, rotation.eulerAngles.y, 0f);

        /*
        tmpRotation.x = Mathf.Min(tmpRotation.x, _maxXAngle);
        tmpRotation.x = Mathf.Max(tmpRotation.x, _minXAngle);
        */
        _pivotCamera.eulerAngles = tmpRotation;
    }


    /// <summary>
    ///  Returns the percent distance between the Camera and the Ball, from a minimum and a maximum value.
    /// </summary>
    /// <returns>Float value between 0 and 1.</returns>
    private float GetDistancePercentCameraToBall()
    {
        float distanceX = (_playerTransform.position.x - _ball.transform.position.x) * (_playerTransform.position.x - _ball.transform.position.x);
        float distanceY = (_playerTransform.position.y - _ball.transform.position.y) * (_playerTransform.position.y - _ball.transform.position.y);
        float distanceZ = (_playerTransform.position.z - _ball.transform.position.z) * (_playerTransform.position.z - _ball.transform.position.z);
        
        float distance = Mathf.Sqrt(distanceX + distanceY + distanceZ); ;

        return GetPercent(distance, _minBallDistance, _maxBallDistance);
    }


    /// <summary>
    /// Set the Fov Value from a parcent from a minimum and a maximum value.
    /// </summary>
    /// <param name="percent">Float between 0 and 1.</param>
    private void SetNewDistanceCameraToPlayer(float percent)
    {
        float value = GetValueFromPercent(percent, _minCameraDistance, _maxCameraDistance);
        transform.localPosition = new Vector3(0f, 3f, value);
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

}
