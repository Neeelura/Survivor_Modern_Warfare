using UnityEngine;

/// <summary>
/// 玩家空闲状态
/// 玩家不移动时的默认状态
/// 继承自 PlayerState_CanAttack，因此空闲时也可以攻击
/// </summary>
public class PlayerState_Idle : PlayerState_CanAttack
{
    public PlayerState_Idle(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void OnUpdate()
    {
        // 先调用基类，处理输入获取和瞄准
        base.OnUpdate();

        // 确保角色不移动（传入 0,0）
        player.DoMove(0, 0);

        // 更新动画为空闲
        player.animController.DoMove(0, 0);

        // 检测到移动输入，切换到移动状态
        if (h != 0 || v != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
