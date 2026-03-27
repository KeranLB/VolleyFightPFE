using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class HumanPlayerInput : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    private Vector3 _forward;
    private Vector3 _right;

    public bool isGamepad;
    public int gamepadIndex;
    
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
        Vector2 currentDirection;
        if(!isGamepad)
        {
            var horizontal = Input.GetKey(KeyCode.A)?-1:Input.GetKey(KeyCode.D)?1:0;
            var vertical = Input.GetKey(KeyCode.W)?1:Input.GetKey(KeyCode.S)?-1:0;
            currentDirection = new Vector2(horizontal, vertical).normalized;
            // currentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            currentDirection = Gamepad.all[gamepadIndex].leftStick.value;
        }
        // Recombine according to original rotation
        var projected = (currentDirection.x * _right + currentDirection.y * _forward).normalized;
        _playerInput.currentDirection = Vector2.right * projected.x + Vector2.up * projected.z;
        
        // Actions
        if(!isGamepad)
        {
            _playerInput.holdsJump = Input.GetKey(KeyCode.Space);
            _playerInput.pressedDoubleJump = Input.GetKeyDown(KeyCode.Space);
            _playerInput.pressedAttack = Mouse.current.leftButton.wasPressedThisFrame;
            _playerInput.pressedBlock = Mouse.current.rightButton.wasPressedThisFrame;
            // _playerInput.holdsJump = Input.GetButton("Jump");
            // _playerInput.pressedAttack = Input.GetButtonDown("Fire1");
            // _playerInput.pressedBlock = Input.GetButtonDown("Fire2");
        }
        else
        {
            _playerInput.holdsJump = Gamepad.all[gamepadIndex].buttonSouth.isPressed;
            _playerInput.pressedDoubleJump = Gamepad.all[gamepadIndex].buttonSouth.isPressed;
            _playerInput.pressedAttack = Gamepad.all[gamepadIndex].buttonWest.wasPressedThisFrame;
            _playerInput.pressedBlock = Gamepad.all[gamepadIndex].buttonEast.wasPressedThisFrame;
        }
    }
}
