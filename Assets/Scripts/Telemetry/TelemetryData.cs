using System;
using UnityEngine;

public abstract class TelemetryData
{
    public abstract string tableName { get; set; }
    
    public abstract string ToJson();
}

[Serializable]
public class TDPlaytest : TelemetryData
{
    public override string tableName {
        get => "Playtests";
        set{}
    }

    public string gameID;
    public string startTime;
    public string endTime;

    public override string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}