using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Player player;
    private Vector3 _aimDirection;
    public float sensitivity = 1f;
    public bool isActive;
    private LineRenderer _aimLineRenderer;
    private Ball _ball;
    public float freezeDuration = 2f;


    private void Awake()
    {
        _aimLineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _aimLineRenderer.enabled = false;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     isActive = !isActive;
        //     if (isActive)
        //     {
        //         StartAim();
        //     }
        //     else
        //     {
        //         EndAim();
        //     }
        // }
        if (!isActive) return;
        
        var movement = player.playerInput.rawDirection;
        _aimDirection = Quaternion.AngleAxis(movement.x * sensitivity * Time.deltaTime, player.playerMovement.characterModel.up) * _aimDirection;
        _aimDirection = Quaternion.AngleAxis(-movement.y * sensitivity * Time.deltaTime, player.playerMovement.characterModel.right) * _aimDirection;
        if (Physics.Raycast(_ball.transform.position, _aimDirection, out RaycastHit hit))
        {
            _aimLineRenderer.SetPosition(0, _ball.transform.position);
            _aimLineRenderer.SetPosition(1, hit.point);
            _aimLineRenderer.material.SetVector("_Center", transform.position);
            player.playerCamera.transform.parent.forward =  _aimDirection;
        }
    }

    public void StartAim(Ball ball, Vector3 initialDirection = default)
    {
        isActive = true;
        _ball = ball;
        player.playerMovement.isFrozen = true;
        if(initialDirection==Vector3.zero)
        {
            _aimDirection = player.playerCamera.transform.forward;
        }
        else
        {
            _aimDirection = initialDirection;
        }
        player.playerMovement.characterModel.forward = _aimDirection;
        _aimLineRenderer.enabled = true;
        Invoke(nameof(EndAim), freezeDuration);
    }

    public void EndAim()
    {
        isActive = false;
        player.playerMovement.isFrozen = false;
        _aimLineRenderer.enabled = false;
        _ball.GetHit(_aimDirection, 0);
        _ball.UnFreeze();
    }
}
