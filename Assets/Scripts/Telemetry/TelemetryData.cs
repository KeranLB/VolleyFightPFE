using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TelemetryData
{
    public static string tableName { get; set; }

    public virtual string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

[Serializable]
public class TDGame : TelemetryData
{
    public static string tableName {
        get => "Game";
        set{}
    }

    public int startTime;
    public int endTime;
}

[Serializable]
public class TDPlayer: TelemetryData
{
    public static string tableName {
        get => "Player";
        set{}
    }

    public string playerId;
    public string controller;
    public int attacks;
    public int blocks;
    public int jumps;
    public int gameId;
}

[Serializable]
public class RecordsListSchema
{
    public List<RecordSchema> records;
}

[Serializable]
public class RecordSchema
{
    public int id;
}
