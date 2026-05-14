using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;

    private TDGame _currentGame;
    public Dictionary<int, TDRound> rounds;
    public Dictionary<int, TDPlayer> players;
    public Dictionary<int, TDActions> actions;
    public Dictionary<int, TDBallExchange> ballExchanges;
    public Dictionary<int, TDBallHit> ballHits;

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
        GameManager.OnGameStarted += StartNewGame;
        GameManager.OnGameEnded += EndGame;
        //PlayerMovement.OnPlayerJump += OnPlayerJump;
        //PlayerMovement.OnPlayerDoubleJump += OnPlayerDoubleJump;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStarted -= StartNewGame;
        GameManager.OnGameEnded -= EndGame;
        //PlayerMovement.OnPlayerJump -= OnPlayerJump;
        //PlayerMovement.OnPlayerDoubleJump -= OnPlayerDoubleJump;
    }

    public void StartNewGame()
    {
        // Init _currentGame
        _currentGame = new TDGame();
        _currentGame.startTime = GetUnixTime();

        // Init Dictionary
        rounds = new Dictionary<int, TDRound>();
        players = new Dictionary<int, TDPlayer>();
        actions = new Dictionary<int, TDActions>();
        ballExchanges = new Dictionary<int, TDBallExchange>();
        ballHits = new Dictionary<int, TDBallHit>();
        
        int playerId = 1;
        
        foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.InstanceID))
        {
            TDPlayer newPlayer = new TDPlayer();
            // Player ID
            newPlayer.playerId = (playerId++).ToString();
            // Controller type
            if (player.TryGetComponent(out HumanPlayerInput humanPlayerInput) && humanPlayerInput.enabled)
            {
                newPlayer.controllerType = humanPlayerInput.isGamepad ? "Gamepad" : "Keyboard";
            }
            else if(player.TryGetComponent(out BotPlayerInput botPlayerInput) && botPlayerInput.enabled)
            {
                newPlayer.controllerType = "Bot";
            }
            else
            {
                continue;
            }
            // Index player
            players.Add(player.gameObject.GetInstanceID(), newPlayer);
        }
    }

    public void EndGame()
    {
        _currentGame.endTime = GetUnixTime();
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
    
    private void OnPlayerJump(int playerId)
    {
        TDPlayer player = players[playerId];
        player.jumps++;
    }
    
    private void OnPlayerDoubleJump(int playerId)
    {
        TDPlayer player = players[playerId];
        player.doubleJumps++;
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
                    result = await TelemetrySender.Instance.SendTelemetry(payload, TDActions.tableName);
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
