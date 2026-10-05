using System;
using UnityEngine;

public class KratosCamera : MonoBehaviour
{
    public Transform pivotX;
    public Transform pivotY;

    [Header("Camera")]
    private Vector3 _smoothVelocity;
    private Ball _ball;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ball = FindFirstObjectByType<Ball>();
    }

    private void Update()
    {
        // Camera orientation
        Vector3 relativePosY = _ball.transform.position - pivotY.position;
        Quaternion rotationY = Quaternion.LookRotation(relativePosY);
        rotationY = Quaternion.Lerp(pivotY.rotation, rotationY, Mathf.Lerp(0.5f, 5f, 1-_ball.currentSpeedLevelIndex/10f) * Time.deltaTime);
        pivotY.eulerAngles= new Vector3(0, rotationY.eulerAngles.y, 0);
        // pivotCamera.rotation = rotation;
        
        // Vector3 relativePosX = _ball.transform.position - pivotX.position;
        // Quaternion rotationX = Quaternion.LookRotation(relativePosX);
        // rotationX = Quaternion.Lerp(pivotX.rotation, rotationX, Mathf.Lerp(0.5f, 5f, 1-_ball.currentSpeedLevelIndex/10f) * Time.deltaTime);
        // pivotX.rotation= Quaternion.LookRotation(relativePosX);
        
        // pivotCamera.eulerAngles = Vector3.SmoothDamp(pivotCamera.eulerAngles, rotation.eulerAngles, ref _smoothVelocity, _smoothTime);
    }
    
}
