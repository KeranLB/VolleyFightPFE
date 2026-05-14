using JetBrains.Annotations;
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
public class TDRound : TelemetryData
{
    public static string tableName
    {
        get => "Round";
        set{}
    }

    public string roundId;
    public int startTime;
    public int endTime;
    public int gameId;
}

[Serializable]
public class TDPlayer : TelemetryData
{
    public static string tableName
    {
        get => "Player";
        set {}
    }

    public string playerId;
    public string controllerType;
    public int jumps;
    public int doubleJumps;
    public int timeAlive;
    public int healthRemaining;
    public int airTime;
    public int slowFallTime;
    public int seeingBallTime;
    public int rounId;
    public int gameId;
}

[Serializable]
public class TDActions : TelemetryData
{
    public static string tableName
    {
        get => "Player";
        set {}
    }

    public string actionId;
    public string actiionType;
    public bool hitBall;
    public bool grouded;
    public bool getHitAfter;
    public int ballSpeed;
    public int time;
    public int roundId;
    public int playerId;
}

[Serializable]
public class TDBallExchange : TelemetryData
{
    public static string tableName
    {
        get => "BallExchange";
        set{}
    }

    public string exchangeId;
    public int speedWhenHit;
    public int damageWhenHit;
    public bool killedPlayer;
    public int time;
    public int gameId;
    public int rounId;
}

[Serializable]
public class TDBallHit : TelemetryData
{
    public static string tableName
    {
        get => "BallHit";
        set{}
    }

    public string playerId;
    public string hitId;
    public int speed;
    public string team;
    public bool teamSwitch;
    public int rebounds;
    public int time;
    public int gameId;
    public int roundId;
    public int exchangeId;
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
