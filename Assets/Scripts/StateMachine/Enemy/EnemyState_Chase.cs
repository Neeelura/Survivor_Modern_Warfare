/// <summary>
/// 敌人的追击状态，生成后直接锁定玩家并持续追击，永不返回巡逻。
/// </summary>
public class EnemyState_Chase : EnemyState_Base
{
    public EnemyState_Chase(EnemyController enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void OnEnter()
    {
        base.OnEnter();
        enemy.animController.DoMove(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // 进入攻击范围则切换攻击
        if (nowTargetSqrDist <= enemy.attackRangeSqr)
        {
            stateMachine.ChangeState(enemy.attackState);
            return;
        }

        // 始终追击玩家
        enemy.DoMove(enemy.player.transform.position);
    }
}
