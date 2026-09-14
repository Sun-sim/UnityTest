using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 8f;

    [Header("竞技场半边长（限制活动范围）")]
    public float arenaHalf = 48f;

    [Header("跳跃")]
    public float jumpForce = 7f;
    public float groundCheckDistance = 1.2f;

    [Header("相机跟随（Lerp 平滑）")]
    public float cameraFollowSpeed = 6f;

    bool isGrounded;
    float fixedCameraY;

    Rigidbody rb;
    Camera cam;
    Vector3 camOffset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cam = Camera.main;
        if (cam != null)
        {
            camOffset = cam.transform.position - transform.position;
            // 记录相机初始高度，之后锁定 Y，避免玩家跳跃时画面上下抖动
            fixedCameraY = cam.transform.position.y;
        }
    }

    void Update()
    {
        // 移动：以相机朝向为基准，投影到 XZ 平面（找不到相机时用世界轴兜底）
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 forward;
        Vector3 right;
        if (cam != null)
        {
            forward = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            right = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
        }
        else
        {
            forward = Vector3.forward;
            right = Vector3.right;
        }
        Vector3 move = (right * h + forward * v);
        if (move.sqrMagnitude > 1) move.Normalize();

        // 地面检测：跳跃前必须确认踩在地面上，防止无限连跳
        isGrounded = CheckGrounded();

        // 跳跃：只改 Y 分量，保留水平速度
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

        rb.velocity = new Vector3(move.x * moveSpeed, rb.velocity.y, move.z * moveSpeed);

        // 鼠标转向：射线打地面，面朝命中点（需相机，找不到则跳过）
        if (cam != null)
        {
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
        }

        // 限制在竞技场内
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -arenaHalf, arenaHalf);
        p.z = Mathf.Clamp(p.z, -arenaHalf, arenaHalf);
        transform.position = p;

        // 相机跟随：Lerp 平滑插值，Y 轴锁死初始高度（跳跃时画面不抖）
        if (cam != null)
        {
            Vector3 targetPos = transform.position + camOffset;
            targetPos.y = fixedCameraY;
            cam.transform.position = Vector3.Lerp(
                cam.transform.position,
                targetPos,
                cameraFollowSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 向下打一条短射线判断是否踩着地面（避免空中无限跳）。
    /// </summary>
    bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance)
               && hit.transform != transform;
    }
}
