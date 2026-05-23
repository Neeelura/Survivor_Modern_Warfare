/// <summary>
/// 敌人的追击状态 当玩家进入敌人的追击范围时，敌人会切换到这个状态，开始追击玩家。
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

        // 与出生点的距离
        float distToSpawnSqr = (enemy.transform.position - enemy.spawnPoint).sqrMagnitude;

        // 超出追击范围，返回出生点
        if (nowTargetSqrDist > enemy.chaseRangeSqr)
        {
            if (distToSpawnSqr > 0.5f)
            {
                enemy.DoMove(enemy.spawnPoint);
            }
            else
            {
                stateMachine.ChangeState(enemy.patrolState);
            }
            return;
        }

        // 追击玩家
        if (nowTargetSqrDist > enemy.attackRangeSqr)
        {
            enemy.DoMove(enemy.player.transform.position);
        }
        // 如果进入攻击范围，切换到攻击状态
        else
        {
            stateMachine.ChangeState(enemy.attackState);
        }
    }
}
