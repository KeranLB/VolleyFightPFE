using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class HumanPlayerInput : MonoBehaviour
{
    private PlayerInput _playerInput;
    
    public Transform cameraPosition;

    public bool isGamepad;
    public int gamepadIndex;
    
    [Header("Camera")]
    [SerializeField] private float _mouseSensitivityX;
    [SerializeField] private float _mouseSensitivityY;
    [SerializeField] private float _joystickSensitivity;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerInput = GetComponent<PlayerInput>();
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
        var projected = (currentDirection.x * cameraPosition.right + currentDirection.y * cameraPosition.forward).normalized;
        _playerInput.currentDirection = Vector2.right * projected.x + Vector2.up * projected.z;

        // Camera Rotation
        Vector2 rotation;
        if (!isGamepad)
        {
            rotation = Input.mousePositionDelta;
            rotation.x *= _mouseSensitivityX;
            rotation.y *= _mouseSensitivityY;
            //rotation = new Vector2(-Input.GetAxis("mouseY"), Input.GetAxis("mouseX"));
        }
        else
        {
            rotation = Gamepad.all[gamepadIndex].rightStick.value * _joystickSensitivity;
        }
        _playerInput.cameraRotation = new Vector3(-rotation.y, rotation.x, 0f);

        // Actions
        if (!isGamepad)
        {
            _playerInput.holdsJump = Input.GetKey(KeyCode.Space);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(PressJumpBuffering());
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                StartCoroutine(ReleaseJumpBuffering());
            }
            _playerInput.pressedAttack = Mouse.current.leftButton.wasPressedThisFrame;
            _playerInput.pressedBlock = Mouse.current.rightButton.wasPressedThisFrame;
            // _playerInput.holdsJump = Input.GetButton("Jump");
            // _playerInput.pressedAttack = Input.GetButtonDown("Fire1");
            // _playerInput.pressedBlock = Input.GetButtonDown("Fire2");
        }
        else
        {
            _playerInput.holdsJump = Gamepad.all[gamepadIndex].buttonSouth.isPressed || Gamepad.all[gamepadIndex].rightTrigger.isPressed;
            if (Gamepad.all[gamepadIndex].buttonSouth.wasPressedThisFrame ||  Gamepad.all[gamepadIndex].rightTrigger.wasPressedThisFrame)
            {
                StartCoroutine(PressJumpBuffering());
            }
            else if (Gamepad.all[gamepadIndex].buttonSouth.wasReleasedThisFrame ||  Gamepad.all[gamepadIndex].rightTrigger.wasReleasedThisFrame)
            {
                StartCoroutine(ReleaseJumpBuffering());
            }
            _playerInput.pressedAttack = Gamepad.all[gamepadIndex].buttonWest.wasPressedThisFrame;
            _playerInput.pressedBlock = Gamepad.all[gamepadIndex].buttonEast.wasPressedThisFrame;
        }
    }
    
    private IEnumerator PressJumpBuffering()
    {
        // Make sure any script that needs to read this value has a chance to execute
        _playerInput.pressedJump = true;
        yield return new WaitForFixedUpdate();
        _playerInput.pressedJump = false;
    }
    
    private IEnumerator ReleaseJumpBuffering()
    {
        // Make sure any script that needs to read this value has a chance to execute
        _playerInput.releasedJump = true;
        yield return new WaitForFixedUpdate();
        _playerInput.releasedJump = false;
    }
}
