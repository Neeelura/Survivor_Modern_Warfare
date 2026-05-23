using UnityEngine;

/// <summary>
/// 经验球 — 拾取后增加经验值
/// </summary>
public class ExpOrb : PickupBase
{
    public int expValue = 10;

    protected override void OnPickup()
    {
        PlayerLevel.Instance.GainExp(expValue);
    }
}
