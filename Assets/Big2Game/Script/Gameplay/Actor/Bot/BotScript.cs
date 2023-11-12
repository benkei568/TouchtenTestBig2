using System.Linq;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BotScript : ParticipantScript
{
    private List<PlayedCardCombination> possibleCombinations = new List<PlayedCardCombination>();

    protected override void Awake()
    {
        base.Awake();
        readyState = new BotReadyState();
        playingState = new BotPlayingState();
        waitingState = new ParticipantWaitingState();
    }

    public override void SetParticipantID(int newID)
    {
        base.SetParticipantID(newID);
        participantName = "Bot " + participantID;
        uIStats.UpdateName(participantName);
    }

    public List<PlayedCardCombination> GetPossibleCombinations()
    {
        return possibleCombinations;
    }

    public void UpdatePossibleCombinations(PlayedCardCombination prevPlayedCard)
    {
        possibleCombinations.Clear();
        var allRuleCard = CardManager.instance.GetAllRuleCards();
        if (!prevPlayedCard.IsCombinationValid())
        {
            for (var i = 0; i < allRuleCard.Count; i++)
            {
                var currentRule = allRuleCard[i];
                var result = GetPermutations(currentCard, currentRule.GetCardCount());
                foreach (var combination in result)
                {
                    var newCombination = new PlayedCardCombination() { cardList = combination.ToList() };
                    if ((currentRule.IsValidCombination(newCombination) && (!GameplayManager.instance.initialSubmit || (GameplayManager.instance.initialSubmit && newCombination.InitialCombination()))))
                    {
                        newCombination.combinationID = i;
                        possibleCombinations.Add(newCombination);
                    }
                }
            }
        }
        else
        {
            var currentRule = allRuleCard[prevPlayedCard.combinationID];
            var result = GetPermutations(currentCard, currentRule.GetCardCount());
            foreach (var combination in result)
            {
                var newCombination = new PlayedCardCombination() { cardList = combination.ToList() };
                if (currentRule.IsValidCombination(newCombination) && currentRule.IsNewHigherRank(prevPlayedCard, newCombination))
                {
                    newCombination.combinationID = prevPlayedCard.combinationID;
                    possibleCombinations.Add(newCombination);
                }
            }
        }
       
    }

    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
    {
        if (length == 1)
        {
            return list.Select(item => new T[] { item });
        }

        return GetPermutations(list, length - 1)
            .SelectMany(item => list.Where(e => !item.Contains(e)),
                (item, newItem) => item.Concat(new T[] { newItem }));
    }

}
