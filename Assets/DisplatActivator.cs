using UnityEngine;

public class DisplatActivator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        
        Display.displays[1].Activate(1920, 1080, 60);
        Display.displays[0].SetRenderingResolution(1920, 1080);
        Display.displays[1].SetRenderingResolution(1920, 1080);

        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
