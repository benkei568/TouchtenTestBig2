public abstract class GameBaseState
{
    public abstract void EnterState(GameplayManager gameplay);
    public abstract void UpdateState(GameplayManager gameplay, float deltaTime);
    public abstract void ExitState(GameplayManager gameplay);

}