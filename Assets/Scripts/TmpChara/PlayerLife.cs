using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class PlayerLife : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public Image LifeBar; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        LifeBar.fillAmount = 1;
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(currentHealth - damage);
    }

    public void ChangeHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        LifeBar.fillAmount = currentHealth/maxHealth;
        Debug.Log("Player health : " + currentHealth);
        if (currentHealth <= 0)
        {
            transform.position = Vector3.up * 50f;
        }
    }

    public void FullHeal()
    {
        ChangeHealth(maxHealth);
        LifeBar.fillAmount = 1;
    }
}
