using System;
using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
    [SerializeField] private Image _crossHairImage;
    [SerializeField] private Camera _camera;
    private Ball _ball;
    
    private void Awake()
    {
        _ball = FindFirstObjectByType<Ball>();
    }

    // Update is called once per frame
    void Update()
    {
        _crossHairImage.transform.position = _camera.WorldToScreenPoint(_ball.transform.position);
    }
}
