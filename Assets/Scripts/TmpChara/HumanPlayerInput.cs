using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class HumanPlayerInput : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    private Vector3 _forward;
    private Vector3 _right;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _forward = transform.forward;
        _right = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        // Movements
        var currentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Recombine according to original rotation
        var projected = (currentDirection.x * _right + currentDirection.y * _forward).normalized;
        _playerInput.currentDirection = Vector2.right * projected.x + Vector2.up * projected.z;
        
        // Actions
        _playerInput.holdsJump = Input.GetButton("Jump");
        _playerInput.pressedAttack = Input.GetButtonDown("Fire1");
        _playerInput.pressedBlock = Input.GetButtonDown("Fire2");
    }
}
