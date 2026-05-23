using UnityEngine;

/// <summary>
/// 玩家攻击状态
/// 支持三段连击：在攻击动画播放期间缓存玩家输入，
/// 动画结束后如果有缓存输入且未到第三段，则切换到下一段攻击。
/// 伤害判定由动画帧事件 OnAttackHit 触发
/// </summary>
public class PlayerState_Attack : PlayerState_Base
{
    // 攻击状态优先级为1
    public override int Priority => 1;

    // 使用 static 保持跨状态切换时的值
    // comboIndex 取值 0/1/2，分别对应第一/二/三段攻击
    private static int comboIndex = 0;

    // 缓存玩家在攻击期间的点击输入
    // true 表示玩家在攻击动画期间按下了鼠标左键
    private static bool pendingComboInput = false;

    public PlayerState_Attack(PlayerController player, StateMachine stateMachine) : base(player, stateMachine) { }

    public override void OnEnter()
    {
        // 重置连击输入缓存（清除上一段攻击的输入记录）
        pendingComboInput = false;

        // 订阅动画帧事件
        // OnAttackHit: 在动画的打击帧触发，用于结算伤害
        // OnAttackEnd: 在动画接近结束时触发，用于切换到下一段或回到 Idle
        player.animController.OnAttackHitEvent += OnAttackHit;
        player.animController.OnAttackEndEvent += OnAttackEnd;

        // 播放对应段数的攻击动画（comboIndex 决定播放哪一段）
        player.animController.DoAttack(comboIndex);
    }

    public override void OnUpdate()
    {
        // 攻击期间禁止移动
        player.DoMove(0, 0);

        // 停止移动动画，播放站立姿态
        player.animController.DoMove(0, 0);

        // 输入缓存机制
        // 检测玩家是否按下了攻击键
        if (Input.GetMouseButtonDown(0))
        {
            pendingComboInput = true;
        }
    }

    public override void OnExit()
    {
        // 取消订阅动画帧事件，防止内存泄漏
        player.animController.OnAttackHitEvent -= OnAttackHit;
        player.animController.OnAttackEndEvent -= OnAttackEnd;
    }

    /// <summary>
    /// 动画帧事件回调：在攻击动画的打击帧触发
    /// 此时武器挥到最前方，执行伤害判定
    /// </summary>
    public void OnAttackHit()
    {
        player.DoAttack(player.dagger);
    }

    /// <summary>
    /// 动画帧事件回调：在攻击动画接近结束时触发
    /// 检查是否有缓存的连击输入，决定切换到下一段还是回到 Idle
    /// </summary>
    public void OnAttackEnd()
    {
        if (pendingComboInput && comboIndex < 2)
        {
            // 有缓存输入且未到第三段，进入下一段攻击
            comboIndex++;
            // 重新进入 Attack 状态，播放下一段动画
            stateMachine.ChangeState(player.attackState);
        }
        else
        {
            // 无缓存输入或已到第三段，重置连击计数，回到 Idle
            comboIndex = 0;
            stateMachine.ChangeState(player.idleState);
        }
    }
}
