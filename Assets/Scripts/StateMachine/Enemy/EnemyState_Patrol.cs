using UnityEngine;

public class EnemyState_Patrol : EnemyState_CanChase
{
    private Vector3 patrolTarget;
    private float waitTimer;
    private bool isWaiting;

    public EnemyState_Patrol(EnemyController enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void OnEnter()
    {
        base.OnEnter();
        // 随机选择一个巡逻点
        PickNewPatrolTarget();
        isWaiting = false;
        enemy.animController.DoMove(true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                PickNewPatrolTarget();
                isWaiting = false;
                enemy.animController.DoMove(true);
            }
            return;
        }

        if ((enemy.transform.position - patrolTarget).sqrMagnitude < 0.5f)
        {
            isWaiting = true;
            waitTimer = Random.Range(1f, 3f);
            enemy.animController.DoMove(false);
            return;
        }

        enemy.DoMove(patrolTarget);
    }

    private void PickNewPatrolTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * enemy.patrolRadius;
        patrolTarget = enemy.spawnPoint + new Vector3(randomCircle.x, 0, randomCircle.y);
    }
}
