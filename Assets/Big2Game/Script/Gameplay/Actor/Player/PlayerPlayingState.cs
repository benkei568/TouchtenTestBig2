

public class PlayerPlayingState : ParticipantPlayingState
{
    public override void EnterState(ParticipantScript player)
    {
        base.EnterState(player);
        //PlayerScript playerScript = player as PlayerScript;
    }

    public override void ExitState(ParticipantScript player)
    {
        //PlayerScript playerScript = player as PlayerScript;
        base.ExitState(player);
    }
}