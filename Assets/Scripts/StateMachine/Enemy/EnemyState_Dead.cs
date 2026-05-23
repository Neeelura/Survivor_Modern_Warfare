using UnityEngine;

public class EnemyState_Dead : EnemyState_Base
{
    public override int Priority => 100;

    public EnemyState_Dead(EnemyController enemy, StateMachine stateMachine) : base(enemy, stateMachine) { }

    // ËÀÍöºó½ûÓÃ Collider + ²¥·ÅËÀÍö¶¯»­
    public override void OnEnter()
    {
        enemy.agent.isStopped = true;
        enemy.GetComponent<Collider>().enabled = false;
        enemy.animController.DoDie();
        enemy.despawnTimer = 2f;
    }

    public override void OnUpdate()
    {
        enemy.despawnTimer -= Time.deltaTime;
        if (enemy.despawnTimer <= 0f)
        {
            enemy.DoDie();
        }
    }
}
