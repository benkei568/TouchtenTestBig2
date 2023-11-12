

public class PlayerPlayingState : ParticipantPlayingState
{
    public override void EnterState(ParticipantScript player)
    {
        base.EnterState(player);
        PlayerScript playerScript = player as PlayerScript;
        playerScript.SetPlayerInteractable(true);
    }

    public override void ExitState(ParticipantScript player)
    {
        PlayerScript playerScript = player as PlayerScript;
        playerScript.SetPlayerInteractable(false);
        base.ExitState(player);
    }
}