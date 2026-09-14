using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// 波次刷怪：自动在场地边缘生成敌人，波次数量与单波数量递增，全部清空后进入下一波。
/// 通过事件 OnWaveStart / OnAllWavesCleared 与 UI 解耦；每生成一个敌人就订阅其 EnemyHealth.OnDeath 来统计存活数。
/// 无需手动拖拽刷怪点预制体——直接在代码中随机边缘布点。
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int totalWaves = 5;
    public int baseCount = 4;
    public float spawnInterval = 0.6f;
    public float waveInterval = 2f;
    public float arenaHalf = 45f;

    public event Action<int> OnWaveStart;     // 参数：当前波次（从 1 开始）
    public event Action OnAllWavesCleared;

    int currentWave;
    int aliveEnemies;    public int CurrentWave => currentWave; // 供 HUD 读取当前波次


    void Start()
    {
        if (enemyPrefab == null)
            Debug.LogWarning("[WaveSpawner] 未指定 enemyPrefab，请在 Inspector 中填入敌人预制体");
        StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        for (currentWave = 1; currentWave <= totalWaves; currentWave++)
        {
            OnWaveStart?.Invoke(currentWave);
            int count = baseCount + (currentWave - 1) * 2;
            aliveEnemies = count;
            for (int i = 0; i < count; i++)
            {
                SpawnOne();
                yield return new WaitForSeconds(spawnInterval);
            }
            while (aliveEnemies > 0)
                yield return null;             // 等本波敌人全部死亡
            yield return new WaitForSeconds(waveInterval);
        }
        OnAllWavesCleared?.Invoke();
    }

    void SpawnOne()
    {
        if (enemyPrefab == null) return;
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float r = arenaHalf * 0.9f;
        Vector3 pos = new Vector3(Mathf.Cos(angle) * r, 1f, Mathf.Sin(angle) * r);
        GameObject e = Instantiate(enemyPrefab, pos, Quaternion.identity);
        var hp = e.GetComponent<EnemyHealth>();
        if (hp != null)
            hp.OnDeath += (score) => { aliveEnemies--; };
    }
}
