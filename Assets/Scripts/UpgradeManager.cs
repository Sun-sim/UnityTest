using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Roguelike 核心：每波结束后（第 1 波除外）暂停游戏，随机抽 3 个强化，玩家按 1/2/3 选择其一。
/// 用 Time.timeScale = 0 暂停一切（物理/协程/Enemy/Bullet 都靠 deltaTime，会被冻结），UI 选择靠 Update 仍可用。
/// 强化直接修改玩家/子弹上的 public 字段，无需重新生成对象。
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    [Serializable]
    public class Upgrade
    {
        public string name;
        public Action apply;
    }

    public Text upgradeText;

    PlayerController player;
    PlayerShooting shooting;
    PlayerHealth health;
    Bullet bulletPrefab;

    List<Upgrade> pool = new List<Upgrade>();
    List<Upgrade> currentChoices = new List<Upgrade>();
    bool choosing;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.GetComponent<PlayerController>();
            shooting = p.GetComponent<PlayerShooting>();
            health = p.GetComponent<PlayerHealth>();
            if (shooting != null && shooting.bulletPrefab != null)
                bulletPrefab = shooting.bulletPrefab.GetComponent<Bullet>();
        }

        // 未手动指定则按名字自动查找升级提示文本
        if (upgradeText == null) upgradeText = GameObject.Find("UpgradeText")?.GetComponent<Text>();
        BuildPool();
        var ws = FindObjectOfType<WaveSpawner>();
        if (ws != null) ws.OnWaveStart += OnWaveStart;
    }

    void BuildPool()
    {
        pool.Add(new Upgrade { name = "1. 移动速度 +20%", apply = () => { if (player != null) player.moveSpeed *= 1.2f; } });
        pool.Add(new Upgrade { name = "2. 射速 +25%",   apply = () => { if (shooting != null) shooting.fireRate *= 0.78f; } });
        pool.Add(new Upgrade { name = "3. 最大生命 +1",  apply = () => { if (health != null) { health.maxHealth += 1; health.currentHealth += 1; } } });
        pool.Add(new Upgrade { name = "4. 子弹伤害 +1",  apply = () => { if (bulletPrefab != null) bulletPrefab.damage += 1; } });
        pool.Add(new Upgrade { name = "5. 子弹速度 +30%", apply = () => { if (bulletPrefab != null) bulletPrefab.speed *= 1.3f; } });
    }

    void OnWaveStart(int wave)
    {
        if (wave <= 1) return; // 第一波不升级
        PresentChoices();
    }

    void PresentChoices()
    {
        choosing = true;
        Time.timeScale = 0f; // 暂停整个世界
        currentChoices = PickThree();
        if (upgradeText != null)
        {
            upgradeText.text = "选择强化（按 1 / 2 / 3）：\n"
                + currentChoices[0].name + "\n"
                + currentChoices[1].name + "\n"
                + currentChoices[2].name;
            upgradeText.enabled = true;
        }
    }

    List<Upgrade> PickThree()
    {
        var picked = new List<Upgrade>();
        var used = new HashSet<int>();
        while (picked.Count < 3)
        {
            int i = UnityEngine.Random.Range(0, pool.Count);
            if (used.Add(i)) picked.Add(pool[i]);
        }
        return picked;
    }

    void Update()
    {
        if (!choosing) return;
        if (Input.GetKeyDown(KeyCode.Alpha1)) Choose(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) Choose(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) Choose(2);
    }

    void Choose(int idx)
    {
        if (idx >= currentChoices.Count) return;
        currentChoices[idx].apply?.Invoke();
        choosing = false;
        Time.timeScale = 1f;
        if (upgradeText != null) upgradeText.enabled = false;
    }
}
