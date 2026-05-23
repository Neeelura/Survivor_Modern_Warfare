using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 中央管理器
/// Canvas 标记 DontDestroyOnLoad，跨场景存活
/// </summary>
public class UIManager
{
    private static UIManager instance = new UIManager();
    public static UIManager Instance => instance;

    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    private Transform canvasTrans;

    private UIManager()
    {
        GameObject canvas = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));
        canvasTrans = canvas.transform;
        Object.DontDestroyOnLoad(canvas);

        // 在 Canvas 下创建伤害飘字管理器
        GameObject dpObj = new GameObject("DamagePopup");
        dpObj.transform.SetParent(canvasTrans, false);
        dpObj.AddComponent<DamagePopup>();

        // 创建武器特效管理器
        GameObject vfxObj = new GameObject("WeaponVFX");
        Object.DontDestroyOnLoad(vfxObj);
        vfxObj.AddComponent<WeaponVFX>();
    }

    /// <summary>
    /// 面板显示
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T ShowPanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;

        GameObject panelObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/" + panelName));
        panelObj.transform.SetParent(canvasTrans, false);

        T panel = panelObj.GetComponent<T>();
        panel.Init();
        panelDic.Add(panelName, panel);
        panel.Show();
        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void HidePanel<T>() where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].Hide();
            Object.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }
}
