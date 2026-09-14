using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public event Action OnDeath;
    public event Action<int> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>敌人/子弹调用，传入伤害量。</summary>
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke();
        }
    }
}
