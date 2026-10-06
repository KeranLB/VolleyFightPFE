using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ProfilerTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float test = Mathf.Clamp01((float) Time.time);
    }
}


public class ButtonUI
{
    public virtual void GetRight()
    {
        
    }
}


public class ButtonSliderUI: ButtonUI
{
    public override void GetRight()
    {
        
    }
}