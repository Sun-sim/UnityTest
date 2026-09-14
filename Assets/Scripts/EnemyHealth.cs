using UnityEngine;
using System;

/// <summary>
/// 敌人血量：被子弹/伤害调用 TakeDamage，归零时触发 OnDeath 并传出得分值。
/// 事件解耦，让 WaveSpawner/计分系统订阅死亡，而不需要敌人知道它们。
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public int scoreValue = 10;

    public event Action<int> OnDeath; // 参数：本敌人得分
    public static event Action<int> OnAnyDeath; // 全局死亡事件，供计分/特效订阅


    int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return; // 已死不再受伤
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke(scoreValue);
            OnAnyDeath?.Invoke(scoreValue); // 全局通知（计分等）
        }
    }
}
