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
        Debug.Log("Player health : " + currentHealth);
        if (currentHealth <= 0)
        {
            transform.position = Vector3.up * 50f;
        }
    }

    public void FullHeal()
    {
        ChangeHealth(maxHealth);
    }
}
