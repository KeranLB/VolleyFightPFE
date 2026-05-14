using System;
using UnityEngine;

public class TelemetryWatcherBall : MonoBehaviour
{
    public Ball ball;

    public int bounces;
    public float exchangeStartTime;
    public int exchangeId;
    public Teams lastTeamTouched;
    public Player lastPlayerTouched;

    private void OnEnable()
    {
        ball.OnBallHit += OnBallHit;
        ball.OnBallBounce += OnBallBounce;
        ball.OnBallPlayer += OnBallPlayer;
    }

    private void OnDisable()
    {
        ball.OnBallHit -= OnBallHit;
        ball.OnBallBounce -= OnBallBounce;
        ball.OnBallPlayer -= OnBallPlayer;
    }

    private void Start()
    {
        exchangeId = 0;
    }

    private void InitExchange()
    {
        bounces = 0;
        exchangeStartTime = Time.time;
        exchangeId++;
        lastTeamTouched = ball.teamPossess;
        lastPlayerTouched = null;
    }

    private void OnBallHit(Player player)
    {
        var playerId = player.gameObject.GetInstanceID();
        var speed = ball.currentSpeedLevelIndex;
        var time = Time.time;
        var switchTeam = lastTeamTouched != player.team;
        lastPlayerTouched = player;
        lastTeamTouched = player.team;
    }

    private void OnBallBounce()
    {
        bounces++;
    }

    private void OnBallPlayer(Player player)
    {
        var playerId = player.gameObject.GetInstanceID();
        var speed = ball.currentSpeedLevelIndex;
        var time = Time.time;
        var exchangeTime = time - exchangeStartTime;
        lastPlayerTouched = player;
        lastTeamTouched = player.team;
    }
    
}
