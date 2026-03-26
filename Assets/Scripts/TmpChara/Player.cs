using System;
using UnityEngine;

[RequireComponent(
    typeof(PlayerInput),
    typeof(PlayerLife),
    typeof(PlayerActions)
)]
public class Player : MonoBehaviour
{
    public Teams team;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public PlayerLife playerLife;
    [HideInInspector]
    public PlayerActions playerActions;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerLife = GetComponent<PlayerLife>();
        playerActions = GetComponent<PlayerActions>();
    }
}
