using System;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Player player;
    private Vector3 aimDirection;
    public float sensitivity = 1f;
    public bool isActive;
    private LineRenderer _aimLineRenderer;


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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isActive = !isActive;
            if (isActive)
            {
                StartAim();
            }
            else
            {
                EndAim();
            }
        }
        if (!isActive) return;
        
        var movement = player.playerInput.rawDirection;
        aimDirection = Quaternion.AngleAxis(movement.x * sensitivity * Time.deltaTime, player.playerMovement.characterModel.up) * aimDirection;
        aimDirection = Quaternion.AngleAxis(-movement.y * sensitivity * Time.deltaTime, player.playerMovement.characterModel.right) * aimDirection;
        if (Physics.Raycast(transform.position, aimDirection, out RaycastHit hit))
        {
            _aimLineRenderer.SetPosition(0, transform.position);
            _aimLineRenderer.SetPosition(1, hit.point);
            _aimLineRenderer.material.SetVector("_Center", transform.position);
        }
    }

    public void StartAim()
    {
        player.playerMovement.isFrozen = true;
        aimDirection = player.playerCamera.transform.forward;
        player.playerMovement.characterModel.forward = aimDirection;
        _aimLineRenderer.enabled = true;
    }

    public void EndAim()
    {
        player.playerMovement.isFrozen = false;
        _aimLineRenderer.enabled = false;
    }
}
