using System;
using TMPro;
using UnityEngine;

public class TelemetrySettings : MonoBehaviour
{
    public TMP_InputField docID;
    public TMP_InputField apiKey;
    public GameObject telemetryPanel;
    
    public static event Action OnTelemetrySettingsChanged;

    public void Open()
    {
        telemetryPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        docID.text = PlayerPrefs.GetString("docId");
        apiKey.text = PlayerPrefs.GetString("apiKey");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetString("docId", docID.text);
        PlayerPrefs.SetString("apiKey", apiKey.text);
        OnTelemetrySettingsChanged?.Invoke();
    }

    public void Close()
    {
        Cursor.visible = false;
        telemetryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (telemetryPanel.activeInHierarchy)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}
