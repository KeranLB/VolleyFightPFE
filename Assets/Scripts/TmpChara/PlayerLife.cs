using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public Image LifeBar;
    public List<Image> LifeBars;

    #region Delegates

    public event Action OnPlayerDeath;

    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        foreach (var item in LifeBars)
        {
            item.fillAmount = 1;
        }
        //LifeBar.fillAmount = 1;
    }

    public void TakeDamage(float damage)
    {
        ChangeHealth(currentHealth - damage);
    }

    public void ChangeHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        foreach (var item in LifeBars)
        {
            item.fillAmount = currentHealth/maxHealth;
        }
        //LifeBar.fillAmount = currentHealth/maxHealth;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        transform.position = Vector3.up * 50f;
        OnPlayerDeath?.Invoke();
    }

    public void FullHeal()
    {
        ChangeHealth(maxHealth);
        foreach (var item in LifeBars)
        {
            item.fillAmount = 1;
        }
        //LifeBar.fillAmount = 1;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
