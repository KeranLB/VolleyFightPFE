using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(PlayerLife))]
public class Player : MonoBehaviour
{
    public Teams team;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public PlayerLife playerLife;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerLife = GetComponent<PlayerLife>();
    }
}
