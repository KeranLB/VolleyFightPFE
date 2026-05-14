using UnityEngine;

public class TelemetryWatcherGame : MonoBehaviour
{
    public float gameStartTime;
    public float gameEndTime;
    public float currentRoundStartTime;
    public float currentRoundEndTime;
    
    private void OnEnable()
    {
        GameManager.OnRoundStarted += OnRoundStarted;
        GameManager.OnRoundEnded += OnRoundEnded;
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameEnded += OnGameEnded;
    }
    
    private void OnDisable()
    {
        GameManager.OnRoundStarted += OnRoundStarted;
        GameManager.OnRoundEnded += OnRoundEnded;
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameEnded += OnGameEnded;
    }

    private void OnRoundStarted()
    {
        currentRoundStartTime = Time.time;
    }

    private void OnRoundEnded()
    {
        currentRoundEndTime = Time.time;
        // Record
    }

    private void OnGameStarted()
    {
        gameStartTime = Time.time;
    }

    private void OnGameEnded()
    {
        gameEndTime = Time.time;
        // Record
    }
}
