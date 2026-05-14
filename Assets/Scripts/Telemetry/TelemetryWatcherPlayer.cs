using UnityEngine;

public class TelemetryWatcherPlayer : MonoBehaviour
{
    public Player player;
    private PlayerActions _playerActions;
    private PlayerMovement _playerMovement;
    private PlayerLife _playerLife;

    public float roundStartTime;
    public float deathTime;
    public int actionId;
    
    public TDPlayer currentPlayer;
    public TDAction currentAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _playerActions = player.playerActions;
        _playerMovement = player.playerMovement;
        _playerLife = player.playerLife;
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
        deathTime = 0f;
        roundStartTime = Time.time;
        InitPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        if(!_playerLife.IsAlive() || currentPlayer==null) return;
        
        if (!_playerMovement.isGrounded)
        {
            currentPlayer.airTime += Time.deltaTime;
            if (_playerMovement.isSlowFalling)
            {
                currentPlayer.slowFallTime += Time.deltaTime;
            }
        }
    }

    private void OnPlayerDeath()
    {
        deathTime = Time.time;
    }

    private void OnPlayerJump()
    {
        currentPlayer.jumps++;
    }

    private void OnPlayerDoubleJump()
    {
        currentPlayer.doubleJumps++;
    }

    private void OnPlayerActionStart()
    {
        currentAction.grounded = _playerMovement.isGrounded;
        currentAction.actionType = _playerActions.activeAbility.abilityType.ToString();
        currentAction.hitBall = false;
        currentAction.hitBallOverlap = false;
        currentAction.ballSpeed = -1;
        currentAction.ballSpeedValue = -1;
        currentAction.time = TelemetryManager.GetUnixTime();
    }

    private void OnPlayerAttackSuccess(Ball ball, bool isOverlap)
    {
        currentAction.ballSpeed = ball.currentSpeedLevelIndex;
        currentAction.ballSpeedValue = ball.GetFinalSpeed();
        currentAction.hitBall = true;
        currentAction.hitBallOverlap = isOverlap;
    }

    private void OnPlayerActionEnd()
    {
        // Record
    }

    private void OnRoundEnded()
    {
        currentPlayer.healthRemaining = _playerLife.currentHealth;
        currentPlayer.timeAlive = (deathTime > 0 ? deathTime : Time.time) - roundStartTime;  
        // Record
    }

    private void InitPlayer()
    {
        currentPlayer = new TDPlayer();
        currentPlayer.roundId = TelemetryManager.roundID;
        currentPlayer.playerId = player.playerId;
        if (player.TryGetComponent(out HumanPlayerInput humanPlayerInput) && humanPlayerInput.enabled)
        {
            currentPlayer.controllerType = humanPlayerInput.isGamepad ? "Gamepad" : "Keyboard";
        }
        else if(player.TryGetComponent(out BotPlayerInput botPlayerInput) && botPlayerInput.enabled)
        {
            currentPlayer.controllerType = "Bot";
        }
        currentPlayer.jumps = 0;
        currentPlayer.doubleJumps = 0;
        currentPlayer.airTime = 0;
        currentPlayer.slowFallTime = 0;
        currentPlayer.seeingBallTime = 0;
        currentPlayer.team = player.team.ToString();

        actionId = 1;
        InitAction();
    }

    private void InitAction()
    {
        currentAction = new TDAction();
        currentAction.roundId = TelemetryManager.roundID;
        currentAction.playerId = player.playerId;
        currentAction.actionId = actionId;
        
        actionId++;
    }
}
