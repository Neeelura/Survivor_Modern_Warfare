using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 结算面板
/// 游戏结束（死亡或通关）时显示
/// 数据从 GameDataManager 读取，不依赖其他面板实例
/// </summary>
public class ResultPanel : BasePanel
{
    [Header("结果展示")]
    public Text txtWaves;
    public Text txtKills;
    public Text txtLevel;

    [Header("历史最佳")]
    public Text txtBestWaves;
    public Text txtBestKills;

    [Header("按钮")]
    public Button btnPlayAgain;
    public Button btnMainMenu;

    public override void Init()
    {
        btnPlayAgain.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
            UIManager.Instance.HidePanel<PlayerHUDPanel>();
            UIManager.Instance.HidePanel<ResultPanel>();
        });

        btnMainMenu.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("BeginScene");
            UIManager.Instance.HidePanel<PlayerHUDPanel>();
            UIManager.Instance.HidePanel<ResultPanel>();
        });
    }

    /// <summary>
    /// 显示结算界面
    /// </summary>
    protected override void OnShow()
    {
        Time.timeScale = 0f;

        int waves = WaveManager.Instance != null ? WaveManager.Instance.CurrentWave : 0;
        int kills = WaveManager.Instance != null ? WaveManager.Instance.TotalKills : 0;
        int level = PlayerLevel.Instance.level;

        txtWaves.text = $"存活波次: {waves} / 25";
        txtKills.text = $"击杀数: {kills}";
        txtLevel.text = $"最高等级: {level}";

        int bestWaves = PlayerPrefs.GetInt("BestWave", 0);
        int bestKills = PlayerPrefs.GetInt("BestKills", 0);

        txtBestWaves.text = $"最佳波次: {bestWaves}";
        txtBestKills.text = $"最佳击杀: {bestKills}";

        if (waves > bestWaves) PlayerPrefs.SetInt("BestWave", waves);
        if (kills > bestKills) PlayerPrefs.SetInt("BestKills", kills);
        PlayerPrefs.Save();
    }
}
