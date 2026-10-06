using System;
using UnityEngine;

[RequireComponent(
    typeof(PlayerInput),
    typeof(PlayerLife),
    typeof(PlayerActions)
)]
[RequireComponent(typeof(KratosMovement))]
public class KratosPlayer : MonoBehaviour
{
    public Teams team;
    
    [Header("Dependencies")]
    public PlayerInput playerInput;
    public PlayerLife playerLife;
    public PlayerActions playerActions;
    public KratosMovement playerMovement;
    public KratosPlayerAim playerAim;
    public KratosCamera playerCamera;

    public int playerId;
}
