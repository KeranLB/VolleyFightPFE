using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 currentDirection;
    public bool holdsJump;
    public bool pressedAttack;
    public bool pressedBlock;
    
    private Vector3 _forward;
    private Vector3 _right;

    private void Start()
    {
        _forward = transform.forward;
        _right = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        // Movements
        currentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Recombine according to original rotation
        var projected = (currentDirection.x * _right + currentDirection.y * _forward).normalized;
        currentDirection = Vector2.right * projected.x + Vector2.up * projected.z;
        
        // Actions
        holdsJump = Input.GetButton("Jump");
        pressedAttack = Input.GetButtonDown("Fire1");
        pressedBlock = Input.GetButtonDown("Fire2");
    }
}
