/// <summary>
/// 玩家移动状态
/// 当玩家按下 WASD 时进入此状态
/// 继承自 PlayerState_CanAttack，因此移动时也可以攻击
/// </summary>
public class PlayerState_Move : PlayerState_CanAttack
{
    public PlayerState_Move(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void OnUpdate()
    {
        // 先调用基类，处理输入获取和瞄准
        base.OnUpdate();

        // 执行移动
        player.DoMove(h, v);

        // 更新移动动画
        player.animController.DoMove(h, v);

        // 如果没有移动输入，切换回空闲状态
        if (h == 0 && v == 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
    }
}
