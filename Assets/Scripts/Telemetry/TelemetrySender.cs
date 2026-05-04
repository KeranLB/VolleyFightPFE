using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TelemetrySender : MonoBehaviour
{
    private string apiKey;
    private string docId;

    public TelemetrySettings telemetrySettings;

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
        
        var td = new TDPlaytest();
        td.gameID = "XXX";
        td.startTime = DateTime.Now.ToLongTimeString();
        td.endTime = DateTime.Now.AddMinutes(3).ToLongTimeString();
        Debug.Log(td.ToJson());

    }

    private async Awaitable CheckSettings()
    {
        docId = PlayerPrefs.GetString("docId", "");
        apiKey = PlayerPrefs.GetString("apiKey", "");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(docId))
        {
            Debug.Log("Doc ID or API Key is empty");
            telemetrySettings.Open();
        }
        else
        {
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
    }

    private async Awaitable<bool> TestConnection()
    {
        string uri = $"https://docs.getgrist.com/api/docs/{docId}";
        using UnityWebRequest www = UnityWebRequest.Get(uri);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);
        
        await www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
        return www.result == UnityWebRequest.Result.Success;

    }

    public string EncapsulateRecord(TelemetryData data)
    {
        return "{\"fields\":"+data.ToJson()+"}";
    }

    public string CollateRecords(TelemetryData[] dataArray)
    {
        string res = "{\"records\":[";
        foreach (var data in dataArray)
        {
            res += data.ToJson() + ",";
        }

        res += "]}";
        return res;
    }

    public IEnumerator SendTelemetry(string payload, string tableId)
    {
        string uri = $"https://docs.getgrist.com/api/docs/{docId}/tables/${tableId}/records";
        using UnityWebRequest www = UnityWebRequest.Post(uri, payload, "application/json");
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", apiKey);
        Debug.Log(www.uploadHandler.data);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log("Data upload complete!");
        }
        Debug.Log(www.downloadHandler.text);
    }
}
