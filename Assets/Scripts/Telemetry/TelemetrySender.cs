using UnityEngine;
using UnityEngine.Networking;

public class TelemetrySender : MonoBehaviour
{
    private string apiKey;
    private string docId;

    public TelemetrySettings telemetrySettings;
    
    public static TelemetrySender Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        TelemetrySettings.OnTelemetrySettingsChanged += async ()=> CheckSettings();
    }

    private void OnDisable()
    {
        TelemetrySettings.OnTelemetrySettingsChanged -= async ()=> CheckSettings();
    }

    private void Start()
    {
        CheckSettings();
    }

    private async Awaitable CheckSettings()
    {
        docId = PlayerPrefs.GetString("docId", "");
        apiKey = PlayerPrefs.GetString("apiKey", "");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(docId))
        {
            Debug.Log("Doc ID or API Key is empty");
            telemetrySettings.Open();
            return;
        }

        if (TelemetrySettings.IsTelemetryEnabled())
        {
            Debug.Log("Testing telemetry connection");
            if (await TestConnection())
            {
                Debug.Log("Connection established");
                telemetrySettings.Close();
            }
            else
            {
                Debug.Log("Connection failed");
            }
        }
        else
        {
            Debug.Log("Telemetry is disabled");
            telemetrySettings.Close();
        }
    }

    private async Awaitable<bool> TestConnection()
    {
        string uri = $"https://pfesmashorpass.getgrist.com/api/docs/{docId}";
        using UnityWebRequest www = UnityWebRequest.Get(uri);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);
        
        await www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        return www.result == UnityWebRequest.Result.Success;

    }

    public async Awaitable<string?> SendTelemetry(string payload, string tableId)
    {
        if (!TelemetrySettings.IsTelemetryEnabled())
        {
            Debug.Log("Telemetry is disabled");
            return null;
        }
        
        string uri = $"https://pfesmashorpass.getgrist.com/api/docs/{docId}/tables/{tableId}/records";
        using UnityWebRequest www = UnityWebRequest.Post(uri, payload, "application/json");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);
        
        Debug.Log("Sending telemetry data");
        Debug.Log(www.uploadHandler.data);
        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error while sending telemetry data");
            Debug.LogError(www.error);
            Debug.Log(www.downloadHandler.text);
            return null;
        }
        
        Debug.Log("Telemetry data sent");
        Debug.Log(www.downloadHandler.text);
        
        return www.downloadHandler.text;
    }
}
