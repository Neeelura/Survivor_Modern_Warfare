using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 伤害飘字管理器
/// 对象池管理飘字实例，动画用 Coroutine + Lerp
/// </summary>
public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Instance { get; private set; }

    [Header("动画参数")]
    public float floatDistance = 1.5f;
    public float duration = 0.8f;
    public float normalFontSize = 24;
    public float critFontSize = 36;
    public Color normalColor = Color.white;
    public Color critColor = Color.red;

    [Header("对象池")]
    public int poolSize = 20;

    // 飘字预制体
    private GameObject popupPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();
    private Transform poolRoot;

    private void Awake()
    {
        Instance = this;
        popupPrefab = Resources.Load<GameObject>("Prefabs/UI/DamagePopup");
    }

    private void Start()
    {
        poolRoot = new GameObject("DamagePopupPool").transform;
        poolRoot.SetParent(transform);

        for (int i = 0; i < poolSize; i++)
            CreateNew();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void CreateNew()
    {
        GameObject obj = Instantiate(popupPrefab, poolRoot);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    private GameObject GetFromPool()
    {
        if (pool.Count == 0)
            CreateNew();
        return pool.Dequeue();
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolRoot);
        pool.Enqueue(obj);
    }

    public static void Show(Vector3 worldPos, int damage, bool isCritical)
    {
        if (Instance == null) return;
        Instance.StartCoroutine(Instance.ShowRoutine(worldPos, damage, isCritical));
    }

    private IEnumerator ShowRoutine(Vector3 worldPos, int damage, bool isCritical)
    {
        GameObject obj = GetFromPool();
        obj.SetActive(true);

        Vector3 screenPos = Camera.main != null
            ? Camera.main.WorldToScreenPoint(worldPos)
            : worldPos;
        obj.transform.position = screenPos;

        Text text = obj.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.text = damage.ToString();
            text.fontSize = isCritical ? (int)critFontSize : (int)normalFontSize;
            text.color = isCritical ? critColor : normalColor;
        }

        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = startPos + Vector3.up * floatDistance;
        Color startColor = isCritical ? critColor : normalColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            obj.transform.position = Vector3.Lerp(startPos, endPos, t);

            if (text != null)
                text.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        ReturnToPool(obj);
    }
}
