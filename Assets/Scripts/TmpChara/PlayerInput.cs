using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInput : MonoBehaviour
{
    public Vector2 currentDirection;
    public bool holdsJump;

    // Update is called once per frame
    void Update()
    {
        currentDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        holdsJump = Input.GetButton("Jump");
    }
}
