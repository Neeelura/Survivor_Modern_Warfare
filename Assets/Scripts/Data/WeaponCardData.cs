/// <summary>
/// 武器卡片数据
/// 用于 WeaponSelectPanel 和 WeaponCard 之间传递武器选项信息
/// </summary>
[System.Serializable]
public class WeaponCardData
{
    public WeaponData weaponData;   // 武器配置 SO
    public bool isUpgrade;          // true = 升级，false = 新武器
    public int currentLevel;        // 当前等级（升级时用）
    public int newLevel;            // 新等级
}
