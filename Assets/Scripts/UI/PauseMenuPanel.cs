using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 暂停菜单面板
/// ESC 键呼出/关闭
/// </summary>
public class PauseMenuPanel : BasePanel
{
    [Header("按钮")]
    public Button btnContinue;
    public Button btnRestart;
    public Button btnMainMenu;

    public override void Init()
    {
        btnContinue.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<PauseMenuPanel>();
            Time.timeScale = 1f;
        });

        btnRestart.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        });

        btnMainMenu.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SaveSystem.SaveRuntimeState();
            SceneManager.LoadScene("BeginScene");
        });
    }

    protected override void OnShow()
    {
        base.OnShow();
        Time.timeScale = 0f; // 暂停游戏
    }

    protected override void OnHide()
    {
        base.OnHide();
        Time.timeScale = 1f; // 恢复游戏
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.HidePanel<PauseMenuPanel>();
        }
    }
}
