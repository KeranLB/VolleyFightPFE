using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class TelemetryData
{
    public static string tableName { get; set; }

    public virtual string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public virtual void ConsolidateData()
    {
        
    }
}

[Serializable]
public class TDGame : TelemetryData
{
    public new static string tableName {
        get => "Game";
        set{}
    }

    public int startTime;
    public int endTime;
}

[Serializable]
public class TDRound : TelemetryData
{
    public new static string tableName
    {
        get => "Round";
        set{}
    }

    public int gameId;
    public int roundId;
    public int startTime;
    public int endTime;

    public override void ConsolidateData()
    {
        gameId = TelemetryManager.gameID;
    }
}

[Serializable]
public class TDPlayer : TelemetryData
{
    public new static string tableName
    {
        get => "Player";
        set {}
    }

    public int gameId;
    public int roundId;
    public int playerId;
    public string controllerType;
    public int jumps;
    public int doubleJumps;
    public float timeAlive;
    public float healthRemaining;
    public float airTime;
    public float slowFallTime;
    public float seeingBallTime;
    public string team;
    
    public override void ConsolidateData()
    {
        gameId = TelemetryManager.gameID;
    }
}

[Serializable]
public class TDAction : TelemetryData
{
    public new static string tableName
    {
        get => "Action";
        set {}
    }

    public int gameId;
    public int roundId;
    public int playerId;
    public int actionId;
    public string actionType;
    public bool hitBall;
    public bool hitBallOverlap;
    public bool grounded;
    public int ballSpeed;
    public float ballSpeedValue;
    public int time;
    
    public override void ConsolidateData()
    {
        gameId = TelemetryManager.gameID;
    }
}

[Serializable]
public class TDBallExchange : TelemetryData
{
    public new static string tableName
    {
        get => "BallExchange";
        set{}
    }

    public int gameId;
    public int roundId;
    public int exchangeId;
    public int playerId;
    public int speed;
    public float speedWhenHit;
    public float damageWhenHit;
    public bool killedPlayer;
    public float duration;
    
    public override void ConsolidateData()
    {
        gameId = TelemetryManager.gameID;
    }
}

[Serializable]
public class TDBallHit : TelemetryData
{
    public new static string tableName
    {
        get => "BallHit";
        set{}
    }

    public int gameId;
    public int roundId;
    public int exchangeId;
    public int hitId;
    public int playerId;
    public int speed;
    public float speedValue;
    public string team;
    public bool teamSwitch;
    public int bounces;
    public int time;
    
    public override void ConsolidateData()
    {
        gameId = TelemetryManager.gameID;
    }
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
