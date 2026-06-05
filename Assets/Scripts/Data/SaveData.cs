using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // 历史最佳（结算时更新）
    public int bestWave;
    public int bestKills;

    // 运行时状态（每波结束时更新，用于"继续游戏"）
    public int currentWave;
    public int totalKills;
    public int playerLevel;
    public int playerExp;
    public List<WeaponSlotData> weaponSlots;

    public SaveData()
    {
        bestWave = 0;
        bestKills = 0;
        currentWave = 0;
        totalKills = 0;
        playerLevel = 1;
        playerExp = 0;
        weaponSlots = new List<WeaponSlotData>();
    }

    /// <summary>
    /// 是否有可继续的游戏进度
    /// </summary>
    public bool CanContinue => currentWave > 0;
}

/// <summary>
/// 武器槽位存档数据，用武器名反查 SO
/// </summary>
[System.Serializable]
public class WeaponSlotData
{
    public string weaponName;
    public int level;
}
