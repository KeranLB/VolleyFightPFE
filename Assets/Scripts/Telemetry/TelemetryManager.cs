using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;
    public static int gameID;
    public static int roundID;

    private TDGame _currentGame;
    public Dictionary<int, TDRound> rounds;
    public Dictionary<int, TDPlayer> players;
    public Dictionary<int, TDAction> actions;
    public Dictionary<int, TDBallExchange> ballExchanges;
    public Dictionary<int, TDBallHit> ballHits;

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
            StartNewGame();
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
        roundID = 1;

        // Init Dictionary
        rounds = new Dictionary<int, TDRound>();
        players = new Dictionary<int, TDPlayer>();
        actions = new Dictionary<int, TDAction>();
        ballExchanges = new Dictionary<int, TDBallExchange>();
        ballHits = new Dictionary<int, TDBallHit>();
    }

    private async Awaitable SendGameData()
    {
        // Game
        string payload = CollateRecords(new TelemetryData[] {_currentGame});
        string result = await TelemetrySender.Instance.SendTelemetry(payload, TDGame.tableName);
        RecordsListSchema recordsPosted = JsonUtility.FromJson<RecordsListSchema>(result);
        if (recordsPosted != null)
        {
            int gameId = recordsPosted.records[0].id;
            foreach (TDPlayer player in players.Values)
            {
                player.gameId = gameId;
            }

            //Rounds
            payload = CollateRecords(rounds.Values.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDRound.tableName);
            if (result != null)
            {
                Debug.Log("All round data sent");

                //Players
                payload = CollateRecords(players.Values.ToArray());
                result = await TelemetrySender.Instance.SendTelemetry(payload, TDPlayer.tableName);
                if (result != null)
                {
                    Debug.Log("All player data sent");

                    //Players Actions
                    payload = CollateRecords(actions.Values.ToArray());
                    result = await TelemetrySender.Instance.SendTelemetry(payload, TDAction.tableName);
                    if (result != null)
                    {
                        Debug.Log("All actions data sent");

                        //Ball Exchanges
                        payload = CollateRecords(ballExchanges.Values.ToArray());
                        result = await TelemetrySender.Instance.SendTelemetry(payload, TDBallExchange.tableName);
                        if (result != null)
                        {
                            Debug.Log("All ball exchanges data sent");

                            //Ball Hits
                            payload = CollateRecords(ballHits.Values.ToArray());
                            result = await TelemetrySender.Instance.SendTelemetry(payload, TDBallHit.tableName);
                            if (result != null)
                            {
                                Debug.Log("All ball hits data sent");
                            }
                            else
                            {
                                Debug.LogError("Could not send ball hits data");
                            }
                        }
                        else
                        {
                            Debug.LogError("Could not send ball exchanges data");
                        }
                    }
                    else
                    {
                        Debug.LogError("Could not send actions data");
                    }
                }
                else
                {
                    Debug.LogError("Could not send player data");
                }
            }
            else
            {
                Debug.LogError("Could not send round data");
            }
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
