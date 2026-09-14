using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 8f;

    [Header("竞技场半边长（限制活动范围）")]
    public float arenaHalf = 49f;

    Rigidbody rb;
    Camera cam;
    Vector3 camOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cam = Camera.main;
        if (cam != null) camOffset = cam.transform.position - transform.position;
    }

    void Update()
    {
        // 移动：以相机朝向为基准，投影到 XZ 平面
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        Vector3 move = (right * h + forward * v);
        if (move.sqrMagnitude > 1) move.Normalize();
        rb.velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);

        // 鼠标转向：射线打地面，面朝命中点
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float dist))
        {
            Vector3 point = ray.GetPoint(dist);
            Vector3 look = point - transform.position;
            look.y = 0;
            if (look.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(look);
        }

        // 限制在竞技场内
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -arenaHalf, arenaHalf);
        p.z = Mathf.Clamp(p.z, -arenaHalf, arenaHalf);
        transform.position = p;

        // 相机跟随（保持初始偏移，平移即可）
        if (cam != null) cam.transform.position = transform.position + camOffset;
    }
}
