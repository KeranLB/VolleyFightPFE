using UnityEngine;

public class TelemetryWatcherBall : MonoBehaviour
{
    public Ball ball;

    public float exchangeStartTime;
    public int exchangeId;
    public int hitId;
    public Teams lastTeamTouched;
    public TDBallExchange currentBallExchange;
    public TDBallHit currentBallHit;

    private void OnEnable()
    {
        ball.OnBallHit += OnBallHit;
        ball.OnBallBounce += OnBallBounce;
        ball.OnBallPlayer += OnBallPlayer;
        
        TelemetryManager.OnRoundStartedTelemetry += OnRoundStarted;
    }

    private void OnDisable()
    {
        ball.OnBallHit -= OnBallHit;
        ball.OnBallBounce -= OnBallBounce;
        ball.OnBallPlayer -= OnBallPlayer;
        
        TelemetryManager.OnRoundStartedTelemetry -= OnRoundStarted;
    }

    private void OnRoundStarted()
    {
        exchangeId = 1;
        InitExchange();
    }

    private void InitExchange()
    {
        currentBallExchange = new TDBallExchange();
        currentBallExchange.roundId = TelemetryManager.roundID;
        currentBallExchange.exchangeId = exchangeId;
        
        exchangeStartTime = Time.time;
        lastTeamTouched = ball.teamPossess;
        
        exchangeId++;
        
        hitId = 1;
        InitHit();
    }

    private void InitHit()
    {
        currentBallHit = new TDBallHit();
        currentBallHit.roundId = TelemetryManager.roundID;
        currentBallHit.exchangeId = exchangeId;
        currentBallHit.hitId = hitId;
        currentBallHit.bounces = 0;
        
        hitId++;
    }

    private void OnBallHit(Player player)
    {
        currentBallHit.playerId = player?.playerId ?? 0; 
        currentBallHit.time = TelemetryManager.GetUnixTime();
        currentBallHit.speed = ball.currentSpeedLevelIndex;
        currentBallHit.speedValue = ball.GetFinalSpeed();
        currentBallHit.team = (player?.team ?? Teams.Neutral).ToString();
        currentBallHit.teamSwitch = lastTeamTouched != (player?.team ?? lastTeamTouched);
        currentBallHit.time = TelemetryManager.GetUnixTime();
        
        // Write record
        TelemetryManager.Instance.ballHits.Add(currentBallHit);
        
        lastTeamTouched = player?.team ?? lastTeamTouched;
        
        InitHit();
    }

    private void OnBallBounce()
    {
        currentBallHit.bounces++;
    }

    private void OnBallPlayer(Player player)
    {
        currentBallExchange.playerId = player.playerId;
        currentBallExchange.speed = ball.currentSpeedLevelIndex;
        currentBallExchange.speedWhenHit = ball.GetFinalSpeed();
        currentBallExchange.damageWhenHit = ball.GetFinalDamage();
        currentBallExchange.killedPlayer = !player.playerLife.IsAlive();
        currentBallExchange.duration = Time.time - exchangeStartTime;
        
        // Write record
        TelemetryManager.Instance.ballExchanges.Add(currentBallExchange);
        
        InitExchange();
    }
    
}
