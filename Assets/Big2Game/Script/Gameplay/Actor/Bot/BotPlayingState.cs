using UnityEngine;

public class BotPlayingState : ParticipantPlayingState
{
    bool doActionTurn = false;
    float actionDelayTimer = 2;

    public override void EnterState(ParticipantScript player)
    {
        base.EnterState(player);
        doActionTurn = false;
        actionDelayTimer = 2;
    }

    public override void UpdateState(ParticipantScript player, GameplayManager gameplay, float deltaTime)
    {
        base.UpdateState(player, gameplay, deltaTime);
        actionDelayTimer -= deltaTime;
        if (!doActionTurn && actionDelayTimer < 0)
        {
            doActionTurn = true;
            BotScript botScript = player as BotScript;
            botScript.UpdatePossibleCombinations(gameplay.GetLastActiveCombination());

            var possibleCombinations = botScript.GetPossibleCombinations();

            if (possibleCombinations.Count == 0)
            {
                botScript.PassTurn();
            }
            else
            {
                botScript.ParticipantSubmit(possibleCombinations[possibleCombinations.Count - 1]);
            }
        }
    }

}