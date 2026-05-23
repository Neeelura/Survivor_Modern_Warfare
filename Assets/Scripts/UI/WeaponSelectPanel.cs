using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器选择面板
/// 每 5 级弹出，展示 3 张武器卡片（新武器或升级）
/// 弹出时游戏暂停，玩家必须选择一把武器后才恢复游戏
///
/// 选项生成规则：
///   - 有空槽位时：优先 2 新武器 + 1 升级
///   - 槽位满 4 时：只出已装备武器的升级选项
/// </summary>
public class WeaponSelectPanel : BasePanel
{
    [Header("卡片 UI")]
    public WeaponCard[] cards = new WeaponCard[3];

    // 武器数据池（从 Resources 加载）
    private List<WeaponData> allWeapons;

    public override void Init()
    {
        // 从 Resources 加载所有远程武器 SO
        allWeapons = new List<WeaponData>();
        WeaponData[] loaded = Resources.LoadAll<WeaponData>("Data/Skills");
        foreach (WeaponData w in loaded)
        {
            if (w.weaponType == WeaponType.Ranged)
                allWeapons.Add(w);
        }
    }

    protected override void OnShow()
    {
        Time.timeScale = 0f;

        List<WeaponCardData> options = GenerateOptions();

        for (int i = 0; i < 3; i++)
        {
            if (i < options.Count)
                cards[i].Setup(options[i], (WeaponCardData data) =>
                {
                    ApplySelection(data);
                });
            else
                cards[i].gameObject.SetActive(false);
        }
    }

    protected override void OnHide()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 生成 3 个武器选项
    /// </summary>
    private List<WeaponCardData> GenerateOptions()
    {
        List<WeaponCardData> options = new List<WeaponCardData>();
        WeaponManager wm = WeaponManager.Instance;
        if (wm == null) return options;

        bool hasEmptySlots = wm.HasEmptySlot();

        // 收集可升级的武器
        List<WeaponSlot> upgradable = new List<WeaponSlot>();
        foreach (WeaponSlot slot in wm.GetEquippedSlots())
        {
            if (slot.CanUpgrade)
                upgradable.Add(slot);
        }

        // 收集未装备的武器
        List<WeaponData> unequipped = new List<WeaponData>();
        foreach (WeaponData weapon in allWeapons)
        {
            if (weapon != null && !wm.IsWeaponEquipped(weapon))
                unequipped.Add(weapon);
        }

        if (hasEmptySlots)
        {
            // 优先新武器，最多 3 张，不够用升级选项补齐
            Shuffle(unequipped);
            for (int i = 0; i < unequipped.Count && options.Count < 3; i++)
            {
                options.Add(new WeaponCardData
                {
                    weaponData = unequipped[i],
                    isUpgrade = false,
                    currentLevel = 0,
                    newLevel = 1,
                });
            }

            Shuffle(upgradable);
            for (int i = 0; i < upgradable.Count && options.Count < 3; i++)
            {
                WeaponSlot slot = upgradable[i];
                options.Add(new WeaponCardData
                {
                    weaponData = slot.weaponData,
                    isUpgrade = true,
                    currentLevel = slot.level,
                    newLevel = slot.level + 1,
                });
            }
        }
        else
        {
            Shuffle(upgradable);
            for (int i = 0; i < 3 && i < upgradable.Count; i++)
            {
                WeaponSlot slot = upgradable[i];
                options.Add(new WeaponCardData
                {
                    weaponData = slot.weaponData,
                    isUpgrade = true,
                    currentLevel = slot.level,
                    newLevel = slot.level + 1,
                });
            }
        }

        return options;
    }

    private void ApplySelection(WeaponCardData data)
    {
        WeaponManager wm = WeaponManager.Instance;
        if (wm == null) return;

        bool success;
        if (data.isUpgrade)
        {
            int slotIndex = -1;
            var equipped = wm.GetEquippedSlots();
            for (int i = 0; i < equipped.Count; i++)
            {
                if (equipped[i].weaponData.weaponName == data.weaponData.weaponName)
                {
                    slotIndex = i;
                    break;
                }
            }
            success = wm.UpgradeWeapon(slotIndex);
        }
        else
        {
            success = wm.AddWeapon(data.weaponData);
        }

        if (success)
        {
            // 刷新 HUD 武器槽位
            PlayerHUDPanel hud = FindObjectOfType<PlayerHUDPanel>();
            if (hud != null) hud.RefreshWeaponSlots();

            UIManager.Instance.HidePanel<WeaponSelectPanel>();
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
