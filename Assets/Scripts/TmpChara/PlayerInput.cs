using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Vector2 currentInput;

    // Update is called once per frame
    void Update()
    {
        currentInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
}
