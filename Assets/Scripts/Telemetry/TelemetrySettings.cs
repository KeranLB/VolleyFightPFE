using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelemetrySettings : MonoBehaviour
{
    public TMP_InputField docID;
    public TMP_InputField apiKey;
    public Toggle sendTelemetry;
    public GameObject telemetryPanel;
    
    public static event Action OnTelemetrySettingsChanged;

    public void Open()
    {
        telemetryPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        docID.text = PlayerPrefs.GetString("docId");
        apiKey.text = PlayerPrefs.GetString("apiKey");
        sendTelemetry.isOn = PlayerPrefs.GetInt("sendTelemetry", 0) == 1;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetString("docId", docID.text);
        PlayerPrefs.SetString("apiKey", apiKey.text);
        PlayerPrefs.SetInt("sendTelemetry", sendTelemetry.isOn ? 1 : 0);
        OnTelemetrySettingsChanged?.Invoke();
    }

    public void Close()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
    
    public static bool IsTelemetryEnabled()
    {
        return PlayerPrefs.GetInt("sendTelemetry", 0) == 1;
    }
}
