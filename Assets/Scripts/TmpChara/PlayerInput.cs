using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInput : MonoBehaviour
{
    public Vector2 currentDirection;
    public Vector3 cameraRotation;
    public bool holdsJump;
    public bool pressedJump;
    public bool releasedJump;
    public bool pressedAttack1;
    public bool pressedAttack2;
    public bool pressedBlock;
}
