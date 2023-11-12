
public abstract class ParticipantBaseState
{
    public abstract void EnterState(ParticipantScript player);
    public abstract void UpdateState(ParticipantScript player, GameplayManager gameplay, float deltaTime);
    public abstract void ExitState(ParticipantScript player);

}