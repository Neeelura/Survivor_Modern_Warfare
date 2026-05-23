using UnityEngine;

/// <summary>
/// 掉落物基类
/// 统一吸附、碰撞检测逻辑，子类需实现 OnPickup() 定义拾取效果
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class PickupBase : MonoBehaviour
{
    [Header("吸附配置")]
    public float moveSpeed = 8f;         // 吸附移动速度
    public float attractDelay = 0.3f;    // 掉落后延迟吸附时间

    // 内部状态
    private bool isAttracted;
    private float spawnTime;

    // 缓存的玩家引用
    protected Transform player;

    private void OnEnable()
    {
        spawnTime = Time.time;
        isAttracted = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.transform;
    }

    private void Update()
    {
        if (Time.time - spawnTime < attractDelay) return;
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float pickupRange = PlayerStats.Instance.TotalPickupRange;

        if (!isAttracted && distToPlayer <= pickupRange)
            isAttracted = true;

        if (isAttracted)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position + Vector3.up,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        OnPickup();
        PoolManager.Instance.Despawn(gameObject);
    }

    /// <summary>
    /// 拾取效果
    /// </summary>
    protected abstract void OnPickup();
}
