using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "savedata.json");

    /// <summary>
    /// 是否处于"继续游戏"模式（MainMenu 设置，WaveManager.Start 读取后立即消费）
    /// </summary>
    public static bool IsContinueMode { get; set; }

    /// <summary>
    /// 从文件加载存档
    /// </summary>
    public static SaveData Load()
    {
        if (!File.Exists(FilePath))
            return new SaveData();

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }
        catch
        {
            return new SaveData();
        }
    }

    /// <summary>
    /// 写入存档到文件
    /// </summary>
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    /// <summary>
    /// 保存运行时游戏状态（每波结束调用）
    /// 合并已有最佳记录后写入
    /// </summary>
    public static void SaveRuntimeState()
    {
        SaveData data = Load();

        if (WaveManager.Instance != null)
        {
            data.currentWave = WaveManager.Instance.CurrentWave;
            data.totalKills = WaveManager.Instance.TotalKills;
        }

        data.playerLevel = PlayerLevel.Instance.level;
        data.playerExp = PlayerLevel.Instance.currentExp;

        data.weaponSlots.Clear();
        if (WeaponManager.Instance != null)
        {
            foreach (WeaponSlot slot in WeaponManager.Instance.slots)
            {
                if (slot != null && slot.weaponData != null)
                {
                    data.weaponSlots.Add(new WeaponSlotData
                    {
                        weaponName = slot.weaponData.weaponName,
                        level = slot.level,
                    });
                }
            }
        }

        Save(data);
    }

    /// <summary>
    /// 清空运行时进度（通关或死亡后调用），保留最佳记录
    /// </summary>
    public static void ClearRuntimeState()
    {
        SaveData data = Load();
        data.currentWave = 0;
        data.totalKills = 0;
        data.playerLevel = 1;
        data.playerExp = 0;
        data.weaponSlots.Clear();
        Save(data);
    }
}
