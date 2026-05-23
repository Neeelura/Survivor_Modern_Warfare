using UnityEngine;

/// <summary>
/// BeginScene 启动器
/// 触发 UIManager 初始化并显示主菜单
/// 挂在 BeginScene 的任意 GameObject 上
/// </summary>
public class MenuBootstrap : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowPanel<MainMenuPanel>();
    }
}
