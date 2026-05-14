using System;
using UnityEngine;

[RequireComponent(
    typeof(PlayerInput),
    typeof(PlayerLife),
    typeof(PlayerActions)
)]
[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
    public Teams team;
    
    [Header("Dependencies")]
    public PlayerInput playerInput;
    public PlayerLife playerLife;
    public PlayerActions playerActions;
    public PlayerMovement playerMovement;

    public int playerId;
}
