using UnityEngine;

/// <summary>
/// UI 面板基类
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 面板初始化
    /// 由 UIManager 在 Instantiate 后、Show() 前调用，只执行一次
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// 显示面板
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public virtual void Hide()
    {
        OnHide();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 面板显示时调用（子类在此注册事件、刷新数据）
    /// </summary>
    protected virtual void OnShow() { }

    /// <summary>
    /// 面板隐藏时调用（子类在此注销事件）
    /// </summary>
    protected virtual void OnHide() { }
}
