using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using static UnityEditor.FilePathAttribute;

public class CenterCamera : MonoBehaviour
{
    #region Components
    [Header("Components :")]
    [Header("")]
    
    [Header("Gameplay Objects :")]
    private GameObject _player;
    [SerializeField] private string _tagPlayer;
    [SerializeField] private Ball _ball;

    [Header("Camera Attachement :")]
    private Camera _camera;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Transform _centerArenaTransform;
    [SerializeField] private Transform _targetBallVision;
    #endregion

    #region Parameters
    [Header("Parameters :")]
    [Header("")]
    [Header("Informations :")]
    [SerializeField] private float _maxDistancePlayerFromCenterArena;

    [Header("")]
    [Header("Pivot Camera Parameters :")]

    [Header("Pivot Camera Rotation :")]
    [SerializeField] private float _slowDampingForce;
    [SerializeField] private float _normalDampingForce;
    
    [Header("Pivot Camera Position :")]
    private float _minDistanceCenterToPivot;
    private float _transitionLimit;
    private float _maxDistanceCenterToPivot;
    private float _distanceSlowFollowPivotCamera;
    [SerializeField] private float _distancePivotFromPlayer;
    [SerializeField] private float _hightPivotFromPlayer;

    [Header("")]
    [Header("Camera Parameters :")]

    [Header("Camera Rotation :")]
    [Tooltip("vers qui la caméra s'oriente le plus (0 = Balle / 1 = Joueur).")]
    [SerializeField][Range(0, 1)] private float _VisionPriority;
    [SerializeField] private float _dampingForcePriorityVision;
    
    [Header("Camera Position")]
    [SerializeField] private float _distanceCameraFromPivot;
    [SerializeField] private float _hightCameraFromPivot;


    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();
        _player = GameObject.FindWithTag(_tagPlayer);
        //_distanceSlowFollowPivotCamera = _minDistanceCenterToPivot;
        DrawDebugCircle(_centerArenaTransform.position + new Vector3(1f, 0f, 0f), _distanceSlowFollowPivotCamera, 360, Color.red);
        InitCameraParameters();
        InitPivotParameters();
    }

    private void InitPivotParameters()
    {
        _distanceSlowFollowPivotCamera = _maxDistancePlayerFromCenterArena / 10f;
        _transitionLimit = _maxDistancePlayerFromCenterArena / 40f;
        _minDistanceCenterToPivot = _distanceSlowFollowPivotCamera + _distancePivotFromPlayer;
        _maxDistanceCenterToPivot = _maxDistancePlayerFromCenterArena + _distancePivotFromPlayer;
    }

    private void InitCameraParameters()
    {
        _camera.transform.position = new Vector3(_camera.transform.position.x, _hightCameraFromPivot, _distanceCameraFromPivot);
    }

    // Update is called once per frame
    void Update()
    {
        //InitCameraParameters();
        //InitPivotParameters();

        CameraRotationFollow();
        NewPivotDistanceToCenter();
        HightFollow();
        KeepBallInVision();
    }

    void DrawDebugCircle(Vector3 center, float radius, int segments, Color color)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0); // Premier point sur l'axe X

        for (int i = 1; i <= segments; i++)
        {
            // Calcul de l'angle en radians
            float angleRad = Mathf.Deg2Rad * (i * angleStep);

            // Nouveau point sur le plan XZ (horizontal / monde 3D)
            // Pour un plan XY (2D), remplacez par : new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0)
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angleRad) * radius, 0, Mathf.Sin(angleRad) * radius);

            // Dessine la ligne entre le point précédent et le nouveau
            Debug.DrawLine(prevPoint, nextPoint, color, 100000000000f);

            // Le point actuel devient le point précédent pour la prochaine itération
            prevPoint = nextPoint;
        }
    }

    private float GetDistance2D(Vector3 positionA, Vector3 positionB)
    {
        float X = (positionB.x - positionA.x) * (positionB.x - positionA.x);
        float Z = (positionB.z - positionA.z) * (positionB.z - positionA.z);
        float result = Mathf.Sqrt(X + Z);
        return result;
    }


    private void CameraRotationFollow()
    {
        float centerToPlayer = GetDistance2D(_player.transform.position, _centerArenaTransform.position);


        if((centerToPlayer > _distanceSlowFollowPivotCamera)&&(centerToPlayer < _transitionLimit))
        {
            float percent = GetPercent(centerToPlayer, _minDistanceCenterToPivot, _maxDistancePlayerFromCenterArena);
            float newDampingForce = GetValueFromPercent(percent, _slowDampingForce, _normalDampingForce);

            Quaternion lookRotation = LookAtTarget(_centerArenaTransform, _player.transform, newDampingForce);
            _centerArenaTransform.eulerAngles = new Vector3(0f, lookRotation.eulerAngles.y, 0f);
        }
        else if (centerToPlayer > _transitionLimit)
        {
            Quaternion lookRotation = LookAtTarget(_centerArenaTransform, _player.transform, _normalDampingForce);
            _centerArenaTransform.eulerAngles = new Vector3(0f, lookRotation.eulerAngles.y, 0f);
        }
        else
        {
            Quaternion lookRotation = LookAtTarget(_centerArenaTransform, _player.transform, _slowDampingForce);
            _centerArenaTransform.eulerAngles = new Vector3(0f, lookRotation.eulerAngles.y, 0f);
        }
        
    }

    private void HightFollow()
    {
        _cameraPivot.position = new Vector3(_cameraPivot.position.x,_player.transform.position.y + _hightPivotFromPlayer, _cameraPivot.position.z);
        
    }

    private void NewPivotDistanceToCenter()
    {
        float distanceCenterToPlayer = GetDistance2D(_centerArenaTransform.position, _player.transform.position);

        if (distanceCenterToPlayer > _minDistanceCenterToPivot)
        {
            float percent = GetPercent(distanceCenterToPlayer, _minDistanceCenterToPivot, _maxDistancePlayerFromCenterArena);
            float newDistancePivotCamera = GetValueFromPercent(percent, _minDistanceCenterToPivot, _maxDistanceCenterToPivot);
            _cameraPivot.localPosition = new Vector3(0f, _cameraPivot.localPosition.y, newDistancePivotCamera);
        }
        else
        {
            _cameraPivot.localPosition = new Vector3(0f, _cameraPivot.localPosition.y, _minDistanceCenterToPivot);
        }
    }

    private void KeepBallInVision()
    {
        _targetBallVision.position = Vector3.Lerp(_ball.transform.position, _player.transform.position,_VisionPriority);
        Quaternion rotation = LookAtTarget(_camera.transform, _targetBallVision, _dampingForcePriorityVision);
       
        _camera.transform.eulerAngles = new Vector3(_camera.transform.eulerAngles.x, rotation.eulerAngles.y, _camera.transform.eulerAngles.z);
    }

    private Quaternion LookAtTarget(Transform from, Transform target, float dampingForce)
    {
        Vector3 relativePos = target.position - from.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation = Quaternion.Lerp(from.rotation, rotation, Mathf.Lerp(dampingForce / 10, dampingForce, 1 - _ball.currentSpeedLevelIndex / 10f) * Time.deltaTime);
        return rotation;
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

        value = Mathf.Clamp(value, min, max);

        float percent = (value - min) / ecart;

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
