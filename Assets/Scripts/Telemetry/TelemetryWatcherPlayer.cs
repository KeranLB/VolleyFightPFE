using System;
using UnityEngine;
using UnityEngine.Serialization;

public class TelemetryWatcherPlayer : MonoBehaviour
{
    public Player player;
    private PlayerActions _playerActions;
    private PlayerMovement _playerMovement;
    private PlayerLife _playerLife;
    private int _playerId;

    public float airTime;
    public float slowFallTime;
    public float roundStartTime;
    public float deathTime;
    public int jumps;
    public int doubleJumps;

    public float currentActionStartTime;
    public AbilityType currentAbilityType;
    public bool currentActionStartedOnGround;
    public bool currentActionHitBall;
    public bool currentActionHitBallOverlap;
    public int currentActionBallSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _playerActions = player.playerActions;
        _playerMovement = player.playerMovement;
        _playerLife = player.playerLife;
        _playerId = player.gameObject.GetInstanceID();
    }

    private void OnEnable()
    {
        player.playerLife.OnPlayerDeath += OnPlayerDeath;
        player.playerMovement.OnPlayerJump += OnPlayerJump;
        player.playerMovement.OnPlayerDoubleJump += OnPlayerDoubleJump;
        player.playerActions.OnPlayerActionStart += OnPlayerActionStart;
        player.playerActions.OnPlayerActionEnd += OnPlayerActionEnd;

        player.playerActions.attack1.attackZone.OnPlayerAttackSuccess += OnPlayerAttackSuccess;
        player.playerActions.attack2.attackZone.OnPlayerAttackSuccess += OnPlayerAttackSuccess;
        player.playerActions.block.attackZone.OnPlayerAttackSuccess += OnPlayerAttackSuccess;

        GameManager.OnRoundStarted += OnRoundStarted;
        GameManager.OnRoundEnded += OnRoundEnded;
    }

    private void OnDisable()
    {
        player.playerLife.OnPlayerDeath -= OnPlayerDeath;
        player.playerMovement.OnPlayerJump -= OnPlayerJump;
        player.playerMovement.OnPlayerDoubleJump -= OnPlayerDoubleJump;
        player.playerActions.OnPlayerActionStart -= OnPlayerActionStart;
        player.playerActions.OnPlayerActionEnd -= OnPlayerActionEnd;
        
        player.playerActions.attack1.attackZone.OnPlayerAttackSuccess -= OnPlayerAttackSuccess;
        player.playerActions.attack2.attackZone.OnPlayerAttackSuccess -= OnPlayerAttackSuccess;
        player.playerActions.block.attackZone.OnPlayerAttackSuccess -= OnPlayerAttackSuccess;
        
        GameManager.OnRoundStarted -= OnRoundStarted;
        GameManager.OnRoundEnded -= OnRoundEnded;
    }

    private void OnRoundStarted()
    {
        airTime = 0f;
        slowFallTime = 0f;
        deathTime = 0f;
        jumps = 0;
        doubleJumps = 0;
        roundStartTime = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!_playerLife.IsAlive()) return;
        
        if (!_playerMovement.isGrounded)
        {
            airTime += Time.deltaTime;
            if (_playerMovement.isSlowFalling)
            {
                slowFallTime += Time.deltaTime;
            }
        }
    }

    private void OnPlayerDeath()
    {
        deathTime = Time.time;
    }

    private void OnPlayerJump()
    {
        jumps++;
    }

    private void OnPlayerDoubleJump()
    {
        doubleJumps++;
    }

    private void OnPlayerActionStart()
    {
        currentActionStartTime = Time.time;
        currentActionStartedOnGround = _playerMovement.isGrounded;
        currentAbilityType = _playerActions.activeAbility.abilityType;
        currentActionHitBall = false;
        currentActionHitBallOverlap = false;
        currentActionBallSpeed = 0;
    }

    private void OnPlayerAttackSuccess(Ball ball, bool isOverlap)
    {
        currentActionBallSpeed = ball.currentSpeedLevelIndex;
        currentActionHitBall = true;
        currentActionHitBallOverlap = isOverlap;
    }

    private void OnPlayerActionEnd()
    {
        // Record
    }

    private void OnRoundEnded()
    {
        var remaingingHealth = _playerLife.currentHealth;
        var timeAlive = (deathTime > 0 ? deathTime : Time.time) - roundStartTime;  
        // Record
    }
}
