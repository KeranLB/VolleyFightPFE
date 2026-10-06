using System;
using System.Collections;
using UnityEngine;

public class KratosPlayerAim : MonoBehaviour
{
    public KratosPlayer player;
    private Vector3 _aimDirection;
    public float sensitivity = 1f;
    public bool isActive;
    private LineRenderer _aimLineRenderer;
    private Ball _ball;
    public float freezeDuration = 2f;
    public float freezeCameraOffset = -5.0f;
    private Coroutine _aimCoroutine;


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
            player.playerCamera.physicalCamera.transform.parent.forward =  _aimDirection;
        }
        
        if (player.playerInput.pressedAttack1)
        {
            StopCoroutine(_aimCoroutine);
            EndAim();
        }
    }

    public void StartAim(Ball ball, Vector3 initialDirection = default)
    {
        isActive = true;
        _ball = ball;
        player.playerMovement.isFrozen = true;
        if(initialDirection==Vector3.zero)
        {
            _aimDirection = player.playerCamera.physicalCamera.transform.forward;
        }
        else
        {
            _aimDirection = initialDirection;
        }
        // player.playerMovement.characterModel.forward = _aimDirection;
        _aimLineRenderer.enabled = true;
        _aimCoroutine = StartCoroutine(AimCoroutine());
        _ball.Freeze();
        player.playerCamera.SetDistanceOffset(freezeCameraOffset);
    }

    private IEnumerator AimCoroutine()
    {
        yield return new WaitForSeconds(freezeDuration);
        EndAim();
    }

    public void EndAim()
    {
        _aimCoroutine = null;
        isActive = false;
        player.playerMovement.isFrozen = false;
        _aimLineRenderer.enabled = false;
        _ball.GetHit(_aimDirection);
        _ball.UnFreeze();
        player.playerCamera.SetDistanceOffset(0.0f);
    }
}
