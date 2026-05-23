using UnityEngine;

/// <summary>
/// 玩家控制器，处理玩家移动、攻击、受击等核心逻辑
/// 实现了 IDamageable 接口，可被敌人攻击
/// </summary>
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("基础属性")]
    public int maxHp = 100;                 // 最大生命值
    public float baseSpeed = 5f;            // 基础移动速度
    public float turnSpeed = 10f;           // 转向速度
    public float basePickupRange = 2f;      // 基础拾取范围
    public float baseAttackRange = 3f;      // 基础攻击索敌范围

    [Header("暴击属性")]
    [Range(0f, 1f)]
    public float critRate = 0.05f;          // 暴击率
    public float critMultiplier = 1.5f;     // 暴击伤害倍率

    public WeaponData dagger;               // 匕首武器数据
    public Transform attackPoint;           // 攻击判定中心点
    public LayerMask enemyLayers;           // 敌人所在的层

    private CharacterController controller;                         // 角色移动控制器
    private Camera mainCamera;                                      // 主摄像机引用
    [HideInInspector] public PlayerAnimController animController;   // 动画控制器引用

    // 伤害判定缓存
    // 预分配数组，避免每帧 new 产生 GC
    private Collider[] hitResults = new Collider[10];

    // FSM 状态机
    public StateMachine FSM { get; private set; }               // 状态机实例
    public PlayerState_Idle idleState { get; private set; }     // 空闲状态
    public PlayerState_Move moveState { get; private set; }     // 移动状态
    public PlayerState_Attack attackState { get; private set; } // 攻击状态

    private void Awake()
    {
        // 初始化状态机和所有状态实例
        FSM = new StateMachine();
        idleState = new PlayerState_Idle(this, FSM);
        moveState = new PlayerState_Move(this, FSM);
        attackState = new PlayerState_Attack(this, FSM);

        // 获取子对象的动画控制器（Animator 所在的子物体）
        animController = GetComponentInChildren<PlayerAnimController>();
    }

    void Start()
    {
        // 获取组件引用
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        // 设置初始状态为空闲
        FSM.ChangeState(idleState);
    }

    void Update()
    {
        // 每帧更新状态机
        FSM.Update();
    }

    /// <summary>
    /// 处理玩家移动（WASD 输入）
    /// </summary>
    /// <param name="h">水平输入（A=-1, D=1）</param>
    /// <param name="v">垂直输入（S=-1, W=1）</param>
    public void DoMove(float h, float v)
    {
        // 计算世界坐标系下的移动方向
        Vector3 worldMoveDir = new Vector3(h, 0, v);

        // 使用 CharacterController 进行移动
        // normalized: 归一化，防止对角线移动速度过快
        // Physics.gravity: 添加重力，防止角色悬浮
        // Time.deltaTime: 每帧时间差，保证移动速度与帧率无关
        controller.Move((worldMoveDir.normalized * PlayerStats.Instance.TotalSpeed + Physics.gravity) * Time.deltaTime);
    }

    /// <summary>
    /// 鼠标瞄准，让角色朝向鼠标位置（平滑旋转）
    /// 使用 Quaternion.Slerp 实现渐进式转向，避免瞬间转向显得突兀
    /// </summary>
    public void DoAim()
    {
        // 从摄像机位置发射一条射线，经过鼠标位置
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // 创建一个水平面（y=0），用于检测射线与地面的交点
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // 计算射线与平面的交点
        if (groundPlane.Raycast(ray, out float enter))
        {
            // 获取交点位置
            Vector3 hitPoint = ray.GetPoint(enter);
            // 保持角色在当前高度，只改变水平朝向
            hitPoint.y = transform.position.y;

            // 计算目标旋转（朝向交点方向）
            Quaternion targetRotation = Quaternion.LookRotation(hitPoint - transform.position);
            // 使用 Slerp 进行平滑旋转
            // turnSpeed * Time.deltaTime: 每帧旋转的角度比例
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 执行攻击判定，对攻击范围内所有敌人造成伤害
    /// 伤害公式：武器基础伤害 × (1+攻击加成) × (1+伤害加成) × 暴击倍率 × 随机波动
    /// </summary>
    /// <param name="weapon">武器数据</param>
    public void DoAttack(WeaponData weapon)
    {
        // 使用 NonAlloc 版本避免 GC
        // 在攻击点周围球体范围内检测敌人
        int hitCount = Physics.OverlapSphereNonAlloc(
            attackPoint.position,      // 检测中心
            weapon.attackRange,        // 检测半径
            hitResults,                // 结果缓存数组
            enemyLayers                // 只检测敌人层
        );

        // 遍历检测到的敌人，逐一结算伤害
        for (int i = 0; i < hitCount; i++)
        {
            IDamageable target = hitResults[i].GetComponent<IDamageable>();
            if (target != null)
            {
                int finalDamage;
                bool isCritical = false;

                finalDamage = PlayerStats.Instance.CalculateDamage(weapon.baseDamage, out isCritical);

                Vector3 hitPos = hitResults[i].transform.position;
                target.TakeDamage(finalDamage, hitPos);
                DamagePopup.Show(hitPos, finalDamage, isCritical);
            }
        }
    }

    /// <summary>
    /// 玩家受到伤害，应用护甲减伤公式
    /// 公式：实际伤害 = 原始伤害 × (100 / (100 + 护甲))
    /// </summary>
    /// <param name="damage">原始伤害值</param>
    /// <param name="hitPoint">受击点位置</param>
    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        // 应用护甲减伤公式
        // Mathf.RoundToInt：四舍五入取整，保证结果为整数
        int actualDamage = Mathf.RoundToInt(damage * (100f / (100f + PlayerStats.Instance.TotalArmor)));

        // 扣除生命值
        PlayerStats.Instance.hp -= actualDamage;

        // 广播事件
        EventCenter.Broadcast<int>("PlayerHpChanged", PlayerStats.Instance.hp);

        // 检查是否死亡
        if (PlayerStats.Instance.hp <= 0)
        {
            DoDie();
        }
    }

    /// <summary>
    /// 玩家死亡逻辑
    /// </summary>
    public void DoDie()
    {
        // 广播死亡事件
        EventCenter.Broadcast("PlayerDied");

        // 通知波次系统和 UI
        UIManager.Instance.ShowPanel<ResultPanel>();
        WaveManager.Instance?.StopGame();
    }

    /// <summary>
    /// 在 Scene 视图中绘制攻击范围球体，方便调参
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null && dagger != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, dagger.attackRange);
        }
    }
}
