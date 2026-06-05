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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameObject.activeSelf)
                UIManager.Instance.HidePanel<PauseMenuPanel>();
            else
                UIManager.Instance.ShowPanel<PauseMenuPanel>();
        }
    }
}
