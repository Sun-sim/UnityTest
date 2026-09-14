using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏总控：按名字自动找到 HUD 文本，订阅玩家血量/敌人死亡/波次事件来刷新显示，处理胜利/失败。
/// 不持有具体敌人或玩家引用，靠事件解耦——这是面试里很加分的“事件驱动”写法。
/// </summary>
public class GameManager : MonoBehaviour
{
    Text scoreText;
    Text healthText;
    Text waveText;

    int score;
    int displayWave;
    bool over;

    PlayerHealth playerHealth;
    WaveSpawner waveSpawner;

    void Start()
    {
        // 按名字找 HUD 文本，避免手动拖引用
        scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        healthText = GameObject.Find("HealthText")?.GetComponent<Text>();
        waveText = GameObject.Find("WaveText")?.GetComponent<Text>();

        playerHealth = GameObject.FindWithTag("Player")?.GetComponent<PlayerHealth>();
        waveSpawner = FindObjectOfType<WaveSpawner>();

        EnemyHealth.OnAnyDeath += (s) => { score += s; Refresh(); };
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += (h) => Refresh();
            playerHealth.OnDeath += () => EndGame(false);
        }
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveStart += (w) => { displayWave = w; Refresh(); };
            waveSpawner.OnAllWavesCleared += () => EndGame(true);
        }
        Refresh();
    }

    void Refresh()
    {
        if (scoreText != null) scoreText.text = "得分: " + score;
        if (healthText != null && playerHealth != null) healthText.text = "生命: " + playerHealth.currentHealth;
        if (waveText != null) waveText.text = "波次: " + displayWave;
    }

    void EndGame(bool win)
    {
        if (over) return;
        over = true;
        if (waveText != null)
            waveText.text = win ? "胜利！得分 " + score : "游戏结束 得分 " + score;
    }
}
