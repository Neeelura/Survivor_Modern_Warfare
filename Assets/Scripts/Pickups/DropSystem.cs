using UnityEngine;

/// <summary>
/// 掉落系统（单例）
/// 管理敌人死亡后的掉落物生成（经验球、医疗包）
///
/// 掉落规则：
///   - 经验球(小)：+10 EXP，概率 80%
///   - 经验球(大)：+50 EXP，概率 15%
///   - 医疗包：恢复 30 HP，概率 5%
/// </summary>
public class DropSystem
{
    private static DropSystem instance = new DropSystem();
    public static DropSystem Instance => instance;

    // 掉落物预制体（构造时从 Resources 加载）
    private GameObject expOrbSmallPrefab;
    private GameObject expOrbLargePrefab;
    private GameObject medkitPrefab;

    // 掉落概率
    public float smallOrbChance = 0.8f;
    public float largeOrbChance = 0.15f;
    public float medkitChance = 0.05f;

    // 掉落散落半径
    public float dropRadius = 1f;

    private DropSystem()
    {
        expOrbSmallPrefab = Resources.Load<GameObject>("Prefabs/Pickups/Exp_Small");
        expOrbLargePrefab = Resources.Load<GameObject>("Prefabs/Pickups/Exp_Large");
        medkitPrefab = Resources.Load<GameObject>("Prefabs/Pickups/Medkit");
    }

    /// <summary>
    /// 在指定位置生成掉落物
    /// 由 EnemyController.DoDie() 调用
    /// </summary>
    public void SpawnDrops(Vector3 position)
    {
        Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
        Vector3 dropPos = position + new Vector3(randomOffset.x, 0, randomOffset.y);

        float roll = Random.value;

        if (roll < medkitChance && medkitPrefab != null)
        {
            PoolManager.Instance.Spawn(medkitPrefab, dropPos, Quaternion.identity);
        }
        else if (roll < medkitChance + largeOrbChance && expOrbLargePrefab != null)
        {
            PoolManager.Instance.Spawn(expOrbLargePrefab, dropPos, Quaternion.identity);
        }
        else if (roll < medkitChance + largeOrbChance + smallOrbChance && expOrbSmallPrefab != null)
        {
            PoolManager.Instance.Spawn(expOrbSmallPrefab, dropPos, Quaternion.identity);
        }
    }
}
