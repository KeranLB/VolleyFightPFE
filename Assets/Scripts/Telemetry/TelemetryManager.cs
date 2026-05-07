using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TelemetryManager : MonoBehaviour
{
    public static TelemetryManager Instance;

    private TDGame _currentGame;
    private Dictionary<int, TDPlayer> _players;

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
        PlayerActions.OnPlayerAttack += OnPlayerAttack;
        AttackZone.OnPlayerAttackSuccess += OnPlayerAttackSuccess;
        PlayerActions.OnPlayerBlock += OnPlayerBlock;
        BlockZone.OnPlayerBlockSuccess += OnPlayerBlockSuccess;
        PlayerMovement.OnPlayerJump += OnPlayerJump;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStarted -= StartNewGame;
        GameManager.OnGameEnded -= EndGame;
        PlayerActions.OnPlayerAttack -= OnPlayerAttack;
        AttackZone.OnPlayerAttackSuccess -= OnPlayerAttackSuccess;
        PlayerActions.OnPlayerBlock -= OnPlayerBlock;
        BlockZone.OnPlayerBlockSuccess -= OnPlayerBlockSuccess;
        PlayerMovement.OnPlayerJump -= OnPlayerJump;
    }

    public void StartNewGame()
    {
        // Init _currentGame
        _currentGame = new TDGame();
        _currentGame.startTime = GetUnixTime();

        // Init _players
        _players = new Dictionary<int, TDPlayer>();
        
        int playerId = 1;
        
        foreach (var player in FindObjectsByType<Player>(FindObjectsSortMode.InstanceID))
        {
            TDPlayer newPlayer = new TDPlayer();
            // Player ID
            newPlayer.playerId = (playerId++).ToString();
            print(newPlayer.playerId);
            // Controller type
            if (player.TryGetComponent(out HumanPlayerInput humanPlayerInput) && humanPlayerInput.enabled)
            {
                newPlayer.controller = humanPlayerInput.isGamepad ? "Gamepad" : "Keyboard";
            }
            else if(player.TryGetComponent(out BotPlayerInput botPlayerInput) && botPlayerInput.enabled)
            {
                newPlayer.controller = "Bot";
            }
            else
            {
                continue;
            }
            // Index player
            _players.Add(player.gameObject.GetInstanceID(), newPlayer);
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

    private void OnPlayerAttack(int playerId)
    {
        print(playerId);
        TDPlayer player = _players[playerId];
        player.attacks++;
    }

    private void OnPlayerAttackSuccess(int playerId)
    {
        TDPlayer player = _players[playerId];
        player.attacksSuccess++;
    }
    
    private void OnPlayerBlock(int playerId)
    {
        TDPlayer player = _players[playerId];
        player.blocks++;
    }

    private void OnPlayerBlockSuccess(int playerId)
    {
        TDPlayer player = _players[playerId];
        player.blocksSuccess++;
    }
    
    private void OnPlayerJump(int playerId)
    {
        TDPlayer player = _players[playerId];
        player.jumps++;
    }

    private async Awaitable SendGameData()
    {
        string payload = CollateRecords(new TelemetryData[] {_currentGame});
        string result = await TelemetrySender.Instance.SendTelemetry(payload, TDGame.tableName);
        RecordsListSchema recordsPosted = JsonUtility.FromJson<RecordsListSchema>(result);
        if (recordsPosted != null)
        {
            int gameId = recordsPosted.records[0].id;
            foreach (TDPlayer player in _players.Values)
            {
                player.gameId = gameId;
            }

            payload = CollateRecords(_players.Values.ToArray());
            result = await TelemetrySender.Instance.SendTelemetry(payload, TDPlayer.tableName);
            if (result != null)
            {
                Debug.Log("All game data sent");
            }
            else
            {
                Debug.LogError("Could not send player data");
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
