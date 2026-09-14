using UnityEngine;

/// <summary>
/// 敌人：朝玩家移动，接触玩家时造成伤害并自毁；被子弹击杀时由 EnemyHealth.OnDeath 统一回收。
/// 用 kinematic Rigidbody + 手动移动，避免敌人之间互相推挤，也能让子弹的 Trigger 命中生效。
/// </summary>
public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int touchDamage = 1;
    public float contactRange = 1.1f;

    Transform player;
    EnemyHealth health;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        health = GetComponent<EnemyHealth>();
        if (health != null) health.OnDeath += OnDied;
    }

    void OnDestroy()
    {
        if (health != null) health.OnDeath -= OnDied;
    }

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void OnDied(int score)
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (player == null) return;
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        float dist = toPlayer.magnitude;
        if (dist > contactRange)
        {
            Vector3 dir = toPlayer / dist;
            transform.rotation = Quaternion.LookRotation(dir);
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(touchDamage);
            if (health != null) health.TakeDamage(health.maxHealth); // 撞击后自毁（走统一死亡流程）
        }
    }
}
