using UnityEngine;

// 可以转为 Chase 状态的类需要继承该类
public class EnemyState_CanChase : EnemyState_Base
{
    public EnemyState_CanChase(EnemyController enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 如果距离小于追击范围，切换到 Chase 状态
        if (nowTargetSqrDist < enemy.chaseRangeSqr)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}
