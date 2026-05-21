using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Player> _players;
    private Dictionary<int, Vector3> _playersSpawnPositions = new();
    private Ball _ball;
    private Vector3 _ballSpawnPosition;
    private Teams _ballStartTeam;

    public static event Action OnGameStarted;
    public static event Action OnGameEnded;
    public static event Action OnRoundStarted;
    public static event Action OnRoundEnded;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init players
        _players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID).ToList();
        int playerId = 1;
        foreach (var player in _players)
        {
            player.playerId = playerId++;
            _playersSpawnPositions.Add(player.playerId, player.transform.position);
        }
        // Init ball
        _ball = FindFirstObjectByType<Ball>();
        _ballSpawnPosition = _ball.transform.position;
        
        // Start round
        OnGameStarted?.Invoke();
        RestartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            OnRoundEnded?.Invoke();
            RestartRound();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            OnGameEnded?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            ResetBall();
        }
    }

    private void RestartRound()
    {
        // Reinit players
        foreach (var player in _players)
        {
            player.transform.position = _playersSpawnPositions[player.playerId];
            player.playerLife.FullHeal();
            player.playerMovement.FullStop();
        }
        // Reinit ball
        ResetBall();
        
        OnRoundStarted?.Invoke();
    }

    private void ResetBall()
    {
        _ball.StopSimulation(_ballSpawnPosition);
        _ball.ChangeTeam(Teams.Neutral);
        _ball.ChangeSpeedLevel(0);
        _ball.GetHit(Vector3.down);
    }
}
