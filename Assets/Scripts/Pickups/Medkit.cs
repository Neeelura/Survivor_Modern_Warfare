using UnityEngine;

/// <summary>
/// 医疗包 — 拾取后恢复生命值
/// </summary>
public class Medkit : PickupBase
{
    public int healAmount = 30;

    protected override void OnPickup()
    {
        PlayerStats.Instance.hp = Mathf.Min(PlayerStats.Instance.hp + healAmount, 100);
        EventCenter.Broadcast<int>("PlayerHpChanged", PlayerStats.Instance.hp);
    }
}
