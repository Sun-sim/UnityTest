using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 玩家射击：按住鼠标左键按 fireRate 开火，使用最简对象池复用子弹（避免频繁 Instantiate/Destroy）。
/// </summary>
public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // 拖入子弹预制体
    public float fireRate = 0.2f;   // 每次射击间隔（秒）

    List<Bullet> pool = new List<Bullet>();
    float cooldown;

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (Input.GetMouseButton(0) && cooldown <= 0f)
        {
            cooldown = fireRate;
            Fire();
        }
    }

    void Fire()
    {
        Bullet b = GetFromPool();
        Vector3 spawn = transform.position + Vector3.up * 0.5f;
        b.transform.position = spawn;
        b.Launch(transform.forward); // 玩家已朝向鼠标
    }

    Bullet GetFromPool()
    {
        // 复用池中未激活的子弹
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }
        // 池空则新建
        GameObject go = Instantiate(bulletPrefab);
        Bullet b = go.GetComponent<Bullet>();
        pool.Add(b);
        go.SetActive(true);
        return b;
    }
}
