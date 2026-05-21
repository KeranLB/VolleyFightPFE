using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;
    public static int gameID;
    public static int roundID;

    public TDGame game;
    public List<TDRound> rounds;
    public List<TDPlayer> players;
    public List<TDAction> actions;
    public List<TDBallExchange> ballExchanges;
    public List<TDBallHit> ballHits;

    #region Delegates

    public static event Action OnGameStartedTelemetry;
    public static event Action OnGameEndedTelemetry;
    public static event Action OnRoundStartedTelemetry;
    public static event Action OnRoundEndedTelemetry;

    #endregion

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
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameEnded += OnGameEnded;
        GameManager.OnRoundStarted += OnRoundStarted;
        GameManager.OnRoundEnded += OnRoundEnded;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameEnded -= OnGameEnded;
        GameManager.OnRoundStarted -= OnRoundStarted;
        GameManager.OnRoundEnded -= OnRoundEnded;
    }

    private void OnGameStarted()
    {
        StartNewGame();
        OnGameStartedTelemetry?.Invoke();
    }

    public void OnGameEnded()
    {
        OnGameEndedTelemetry?.Invoke();
        if (TelemetrySettings.IsTelemetryEnabled())
        {
            SendGameData();
        }
        else
        {
            Debug.Log("Telemetry disabled, skipping sending game data");
        }
    }
    
    private void OnRoundStarted()
    {
        roundID++;
        OnRoundStartedTelemetry?.Invoke();
    }

    private void OnRoundEnded()
    {
        OnRoundEndedTelemetry?.Invoke();
    }
    
    public void StartNewGame()
    {
        roundID = 0;

        // Init Dictionary
        rounds = new List<TDRound>();
        players = new List<TDPlayer>();
        actions = new List<TDAction>();
        ballExchanges = new List<TDBallExchange>();
        ballHits = new List<TDBallHit>();
    }

    private async Awaitable SendGameData()
    {
        // Game
        string payload = CollateRecords(new TelemetryData[] {game});
        string result = await TelemetrySender.Instance.SendTelemetry(payload, TDGame.tableName);
        RecordsListSchema recordsPosted = JsonUtility.FromJson<RecordsListSchema>(result);
        if (recordsPosted != null)
        {
            gameID = recordsPosted.records[0].id;
            
            //Rounds
            payload = CollateRecords(rounds.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDRound.tableName);
            if (result != null)
            {
                Debug.Log("All round data sent");
            }
            else
            {
                Debug.LogError("Could not send round data");
            }
            //Players
            payload = CollateRecords(players.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDPlayer.tableName);
            if (result != null)
            {
                Debug.Log("All player data sent");
            }
            else
            {
                Debug.LogError("Could not send player data");
            }
            //Players Actions
            payload = CollateRecords(actions.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDAction.tableName);
            if (result != null)
            {
                Debug.Log("All actions data sent");
            }
            else
            {
                Debug.LogError("Could not send actions data");
            }
            //Ball Exchanges
            payload = CollateRecords(ballExchanges.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDBallExchange.tableName);
            if (result != null)
            {
                Debug.Log("All ball exchanges data sent");
            }
            else
            {
                Debug.LogError("Could not send ball exchanges data");
            }
            //Ball Hits
            payload = CollateRecords(ballHits.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDBallHit.tableName);
            if (result != null)
            {
                Debug.Log("All ball hits data sent");
            }
            else
            {
                Debug.LogError("Could not send ball hits data");
            }
            Debug.Log("End of data sending");
        }
        else
        {
            Debug.LogError("Could not send game data");
        }
        StartNewGame();
    }
    
    public static string EncapsulateRecord(TelemetryData data)
    {
        return "{\"fields\":"+data.ToJson()+"}";
    }

    public static string CollateRecords(TelemetryData[] dataArray)
    {
        if (dataArray.Length == 0) return "";
        
        string res = "{\"records\":[";
        foreach (var data in dataArray)
        {
            data.ConsolidateData();
            res += EncapsulateRecord(data) + ",";
        }
        res = res.Remove(res.Length - 1, 1);
        res += "]}";
        return res;
    }

    public static int GetUnixTime()
    {
        return (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }


}
