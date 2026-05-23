using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 单张武器卡片 UI 组件
/// 挂载到武器选择面板中的每张卡片 Prefab 上
/// </summary>
public class WeaponCard : MonoBehaviour
{
    [Header("UI 组件")]
    public Image icon;
    public Text txtName;
    public Text txtLevel;
    public Text txtDesc;            // 特效描述
    public Text txtAttribute;       // 属性加成文字
    public Button btnSelect;

    private WeaponCardData cardData;

    public void Setup(WeaponCardData data, System.Action<WeaponCardData> callback)
    {
        cardData = data;

        if (txtName != null)
            txtName.text = data.weaponData.weaponName;

        if (icon != null && data.weaponData.icon != null)
            icon.sprite = data.weaponData.icon;

        if (txtLevel != null)
        {
            txtLevel.text = data.isUpgrade
                ? $"Lv.{data.currentLevel} → Lv.{data.newLevel}"
                : "Lv.1";
        }

        if (txtDesc != null)
        {
            WeaponEffect eff = data.weaponData.weaponEffect;
            int param = data.weaponData.effectParam;
            txtDesc.text = eff switch
            {
                WeaponEffect.Penetrate => $"穿透 {param} 个目标",
                WeaponEffect.Scatter => $"散射 {param} 颗子弹",
                WeaponEffect.AOE => $"爆炸半径 {param}m",
                WeaponEffect.DOT => $"灼烧 {param} 秒",
                _ => "无特殊效果",
            };
        }

        if (txtAttribute != null)
        {
            int lv = data.isUpgrade ? data.newLevel : 1;
            WeaponLevelData ld = data.weaponData.GetLevelData(lv);
            string attrs = "";
            if (ld.bonusArmor != 0) attrs += $"护甲 +{ld.bonusArmor} ";
            if (ld.bonusSpeed > 0) attrs += $"移速 +{ld.bonusSpeed * 100f:F0}% ";
            if (ld.bonusDamage > 0) attrs += $"伤害 +{ld.bonusDamage * 100f:F0}% ";
            if (ld.bonusAttackRange > 0) attrs += $"范围 +{ld.bonusAttackRange * 100f:F0}% ";
            if (ld.bonusPickupRange > 0) attrs += $"拾取 +{ld.bonusPickupRange * 100f:F0}% ";
            txtAttribute.text = attrs.Trim();
        }

        if (btnSelect != null)
            btnSelect.onClick.AddListener(() =>
            {
                callback?.Invoke(cardData);
            });
    }
}
