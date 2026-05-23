using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单面板
/// </summary>
public class MainMenuPanel : BasePanel
{
    [Header("按钮")]
    public Button btnStart;
    public Button btnContinue;
    public Button btnQuit;

    public override void Init()
    {
        btnStart.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<PlayerHUDPanel>();
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.HidePanel<MainMenuPanel>();
        });

        btnContinue.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<PlayerHUDPanel>();
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.HidePanel<MainMenuPanel>();
        });

        btnQuit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
