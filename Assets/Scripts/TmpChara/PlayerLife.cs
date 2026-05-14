using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public Image LifeBar;

    #region Delegates

    public event Action OnPlayerDeath;

    #endregion
    
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
        LifeBar.fillAmount = 1;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
}
