using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;

    [SerializeField] private List<GameObject> _lifeBars;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject element in _lifeBars)
        {
            element.transform.LookAt(_cameraPosition);
            element.transform.eulerAngles = new Vector3(0f,element.transform.eulerAngles.y,0f);
        }
    }
}
