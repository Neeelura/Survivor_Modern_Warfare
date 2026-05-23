public abstract class PlayerState_Base : IState
{
    protected PlayerController player;
    protected StateMachine stateMachine;

    public virtual int Priority => 0;

    public PlayerState_Base(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
}
