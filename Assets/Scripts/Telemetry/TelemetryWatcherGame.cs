using UnityEngine;

public class TelemetryWatcherGame : MonoBehaviour
{
    public TDGame currentGame;
    public TDRound currentRound;
    
    private void OnEnable()
    {
        TelemetryManager.OnRoundStartedTelemetry += OnRoundStarted;
        TelemetryManager.OnRoundEndedTelemetry += OnRoundEnded;
        TelemetryManager.OnGameStartedTelemetry += OnGameStarted;
        TelemetryManager.OnGameEndedTelemetry += OnGameEnded;
    }
    
    private void OnDisable()
    {
        TelemetryManager.OnRoundStartedTelemetry -= OnRoundStarted;
        TelemetryManager.OnRoundEndedTelemetry -= OnRoundEnded;
        TelemetryManager.OnGameStartedTelemetry -= OnGameStarted;
        TelemetryManager.OnGameEndedTelemetry -= OnGameEnded;
    }

    private void OnRoundStarted()
    {
        InitRound();
    }

    private void OnRoundEnded()
    {
        currentRound.endTime = TelemetryManager.GetUnixTime();
        
        // Write record
        TelemetryManager.Instance.rounds.Add(currentRound);
    }

    private void OnGameStarted()
    {
        InitGame();
    }

    private void OnGameEnded()
    {
        currentGame.endTime = TelemetryManager.GetUnixTime();
        
        // Write record
        TelemetryManager.Instance.game = currentGame;
    }

    public void InitGame()
    {
        currentGame = new TDGame();
        currentGame.startTime = TelemetryManager.GetUnixTime();
    }

    public void InitRound()
    {
        currentRound = new TDRound();
        currentRound.roundId = TelemetryManager.roundID;
        currentRound.startTime = TelemetryManager.GetUnixTime();
        
    }
}
