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
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public PlayerLife playerLife;
    [HideInInspector]
    public PlayerActions playerActions;
    [HideInInspector]
    public PlayerMovement playerMovement;

    public int playerId;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerLife = GetComponent<PlayerLife>();
        playerActions = GetComponent<PlayerActions>();
        playerMovement = GetComponent<PlayerMovement>();
    }
}
