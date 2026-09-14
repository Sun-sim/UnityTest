using UnityEngine;

/// <summary>
/// 子弹：手动沿 forward 方向移动，碰到带 EnemyHealth 的物体就造成伤害并回收到池。
/// 用 kinematic Rigidbody + Trigger 碰撞器，配合敌人（非 Trigger 碰撞器 + Rigidbody）触发 OnTriggerEnter。
/// </summary>
public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifeTime = 2f;

    float timer;

    void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;       // 手动控制位移，不受重力/物理推动
        var col = GetComponent<SphereCollider>();
        if (col != null) col.isTrigger = true;        // 用 Trigger 检测命中而非物理碰撞
    }

    /// <summary>由对象池取出后调用，设定飞行方向。</summary>
    public void Launch(Vector3 direction)
    {
        timer = lifeTime;
        if (direction.sqrMagnitude > 0.0001f)
            transform.forward = direction;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer <= 0f) Return();
    }

    void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Return();
        }
    }

    /// <summary>回收：隐藏物体，等待对象池再次取出复用。</summary>
    public void Return()
    {
        gameObject.SetActive(false);
    }
}
