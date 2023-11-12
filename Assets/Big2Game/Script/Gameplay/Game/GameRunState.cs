public class GameRunState : GameBaseState
{
    public override void EnterState(GameplayManager gameplay)
    {

    }

    public override void ExitState(GameplayManager gameplay)
    {

    }

    public override void UpdateState(GameplayManager gameplay, float deltaTime)
    {
        foreach (var participant in gameplay.participantList)
        {
            participant.GameplayUpdate(gameplay, deltaTime);
        }
    }
}