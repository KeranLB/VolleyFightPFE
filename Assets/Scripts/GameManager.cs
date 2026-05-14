using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Player> _players;
    private Dictionary<Player, Vector3> _playersSpawnPositions = new();
    private Ball _ball;
    private Vector3 _ballSpawnPosition;
    private Teams _ballStartTeam;
    private FieldForce _fieldForce;

    public static event Action OnGameStarted;
    public static event Action OnRoundStarted;
    public static event Action OnRoundEnded;
    public static event Action OnGameEnded;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Init players
        _players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID).ToList();
        foreach (var player in _players)
        {
            _playersSpawnPositions.Add(player, player.transform.position);
        }
        // Init ball
        _ball = FindFirstObjectByType<Ball>();
        _ballSpawnPosition = _ball.transform.position;
        _ballStartTeam = _ball.teamPossess;
        // Init FieldForce
        _fieldForce = FindFirstObjectByType<FieldForce>();
        
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
    }

    private void RestartRound()
    {
        // Reinit players
        foreach (var player in _players)
        {
            player.transform.position = _playersSpawnPositions[player];
            player.playerLife.FullHeal();
            player.playerMovement.FullStop();
        }
        // Reinit ball
        _ball.StopSimulation(_ballSpawnPosition);
        _ball.ChangeTeam(_ballStartTeam);
        _ball.ChangeSpeedLevel(0);
        _ball.GetHit(Vector3.down);
        // Reinit field force
        _fieldForce?.ChangeTeam(Teams.Neutral);
        
        OnRoundStarted?.Invoke();
    }
}
