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

    protected override void Start()
    {
        base.Start();
        uIStats.UpdateCharacterSprite((GameManager.instance.characterSprite + participantID) % GameplayManager.instance.participantList.Count);
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
                    newCombination = ValidateCombination(currentRule, newCombination);
                    if ((newCombination.IsCombinationValid() && (!GameplayManager.instance.initialSubmit || (GameplayManager.instance.initialSubmit && newCombination.InitialCombination()))))
                    {
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
                newCombination = ValidateCombination(currentRule, newCombination);
                if (newCombination.IsCombinationValid() && newCombination.combinationID == prevPlayedCard.combinationID && currentRule.IsNewHigherRank(prevPlayedCard, newCombination))
                {
                    possibleCombinations.Add(newCombination);
                }
            }
        }
       
    }

    PlayedCardCombination ValidateCombination(RuleCardCombination ruleCard, PlayedCardCombination cardCombination)
    {
        var result = ruleCard.IsValidCombination(cardCombination);
        if (result.isValid)
        {
            cardCombination.combinationID = ruleCard.ruleID;
            cardCombination.combinationEnum = result.fiveCombinationEnum;
            cardCombination.customComparisonValue = result.comparisonValue;
            return cardCombination;
        }
        cardCombination.combinationID = -1;
        cardCombination.combinationEnum = FiveCombinationEnum.none;
        cardCombination.customComparisonValue = -1;
        return cardCombination;
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
