using System;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(currentHealth - damage);
    }

    public void ChangeHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Player health: " + currentHealth);
    }
}
