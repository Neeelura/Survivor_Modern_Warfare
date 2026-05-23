using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器视觉特效管理器
/// 管理子弹飞行、爆炸等视觉特效的对象池
/// 子弹做直线运动飞向目标，与伤害判定无关
/// </summary>
public class WeaponVFX : MonoBehaviour
{
    public static WeaponVFX Instance { get; private set; }

    [Header("子弹飞行速度")]
    public float bulletSpeed = 20f;         // 普通子弹/火箭弹飞行速度
    public float flameDuration = 0.6f;      // 火焰持续时间

    // 子弹对象池（按 prefab.name 分组）
    private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
    private GameObject poolRoot;

    private void Awake()
    {
        Instance = this;
        poolRoot = new GameObject("WeaponVFXPool");
        poolRoot.transform.SetParent(transform);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        string key = prefab.name;
        if (!pool.ContainsKey(key))
            pool[key] = new Queue<GameObject>();

        GameObject obj;
        if (pool[key].Count > 0)
            obj = pool[key].Dequeue();
        else
            obj = Instantiate(prefab);

        obj.SetActive(true);
        return obj;
    }

    private void ReturnToPool(GameObject obj, GameObject prefab)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolRoot.transform);

        string key = prefab.name;
        if (!pool.ContainsKey(key))
            pool[key] = new Queue<GameObject>();
        pool[key].Enqueue(obj);
    }

    /// <summary>
    /// 发射一颗子弹：从发射点直线飞向目标，到达后消失
    /// 用于手枪、步枪、霰弹枪、狙击枪
    /// </summary>
    public void FireBullet(GameObject prefab, Vector3 start, Vector3 end)
    {
        if (prefab == null) return;
        StartCoroutine(BulletRoutine(prefab, start, end, null));
    }

    /// <summary>
    /// 发射火箭弹：飞到目标后播放爆炸特效
    /// 用于火箭筒
    /// </summary>
    public void FireRocket(GameObject bulletPrefab, GameObject explosionPrefab, Vector3 start, Vector3 end)
    {
        if (bulletPrefab == null) return;
        StartCoroutine(BulletRoutine(bulletPrefab, start, end, explosionPrefab));
    }

    /// <summary>
    /// 喷射火焰：在起点向目标方向生成火焰特效，持续 flameDuration 秒
    /// 用于火焰喷射器
    /// </summary>
    public void FireFlame(GameObject flamePrefab, Vector3 start, Vector3 target)
    {
        if (flamePrefab == null) return;
        StartCoroutine(FlameRoutine(flamePrefab, start, target));
    }

    /// <summary>
    /// 协程
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="explosionPrefab"></param>
    /// <returns></returns>
    private IEnumerator BulletRoutine(GameObject prefab, Vector3 start, Vector3 end, GameObject explosionPrefab)
    {
        GameObject bullet = GetFromPool(prefab);
        bullet.transform.position = start;

        // 朝向目标
        Vector3 dir = (end - start).normalized;
        if (dir != Vector3.zero)
            bullet.transform.forward = dir;

        float dist = Vector3.Distance(start, end);
        float duration = dist / bulletSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            bullet.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        bullet.transform.position = end;

        // 爆炸特效
        if (explosionPrefab != null)
        {
            GameObject explosion = GetFromPool(explosionPrefab);
            explosion.transform.position = end;
            explosion.SetActive(true);
            // 爆炸持续一段时间后回收（这里用固定延时，粒子系统可自播放）
            StartCoroutine(RecycleAfterDelay(explosion, explosionPrefab, 1f));
        }

        ReturnToPool(bullet, prefab);
    }

    private IEnumerator FlameRoutine(GameObject prefab, Vector3 start, Vector3 target)
    {
        GameObject flame = GetFromPool(prefab);
        flame.transform.position = start;

        // 朝向目标
        Vector3 dir = (target - start).normalized;
        if (dir != Vector3.zero)
            flame.transform.forward = dir;

        yield return new WaitForSeconds(flameDuration);

        ReturnToPool(flame, prefab);
    }

    private IEnumerator RecycleAfterDelay(GameObject obj, GameObject prefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(obj, prefab);
    }
}
