using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public string cardPrefabID;
    [SerializeField]
    private CardDictionarySO cardDictionary;
    private List<RuleCardCombination> ruleCardCombinationsList;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        ruleCardCombinationsList = new List<RuleCardCombination>()
        {
            new SingleCombination(), new PairsCombination(), new TriplesCombination(), new FiveCombination()
        };
        for (var i = 0; i < ruleCardCombinationsList.Count; i++)
        {
            ruleCardCombinationsList[i].ruleID = i;
        }
    }

    public CardDictionarySO GetDictionarySO()
    {
        return cardDictionary;
    }

    public PlayedCardCombination ValidateCombination(PlayedCardCombination playedCard)
    {
        for (var i = 0; i < ruleCardCombinationsList.Count; i++)
        {
            var result = ruleCardCombinationsList[i].IsValidCombination(playedCard);
            if (result.isValid)
            {
                playedCard.combinationID = i;
                playedCard.combinationEnum = result.fiveCombinationEnum;
                playedCard.customComparisonValue = result.comparisonValue;
                return playedCard;
            }
        }
        playedCard.combinationID = -1;
        playedCard.combinationEnum = FiveCombinationEnum.none;
        playedCard.customComparisonValue = -1;
        return playedCard;
    }

    public bool IsCombinationHigherValue(PlayedCardCombination prevCombination, PlayedCardCombination newCombination)
    {
        if (prevCombination.combinationID != newCombination.combinationID || prevCombination.combinationID < 0) return false;
        return ruleCardCombinationsList[prevCombination.combinationID].IsNewHigherRank(prevCombination, newCombination);
    }

    public List<RuleCardCombination> GetAllRuleCards()
    {
        return ruleCardCombinationsList;
    }
}

public class PlayedCardCombination
{
    public int combinationID;
    public List<int> cardList;
    public FiveCombinationEnum combinationEnum;
    public int customComparisonValue;

    public PlayedCardCombination()
    {
        combinationID = -1;
        cardList = new List < int>();
        combinationEnum = FiveCombinationEnum.none;
        customComparisonValue = -1;
    }

    public bool IsCombinationValid()
    {
        return combinationID >= 0;
    }

    public bool InitialCombination()
    {
        return cardList.Contains(0);
    }

    public CardData GetCardData(int index)
    {
        return CardManager.instance.GetDictionarySO().GetCardByID(cardList[index]);
    }

    public List<CardData> GetCardDataList()
    {
        List<CardData> cardDataList = new List<CardData>();
        foreach (var card in cardList)
        {
            cardDataList.Add(CardManager.instance.GetDictionarySO().GetCardByID(card));
        }
        return cardDataList;
    }

}

public abstract class RuleCardCombination
{
    public int ruleID;
    public abstract int GetCardCount();

    public virtual CombinationValidationResult IsValidCombination(PlayedCardCombination playedCard)
    {
        return new CombinationValidationResult() { isValid = playedCard.cardList.Count == GetCardCount(), fiveCombinationEnum = FiveCombinationEnum.none, comparisonValue = 0 };
    }

    public abstract bool IsNewHigherRank(PlayedCardCombination prevPlayedCard, PlayedCardCombination newPlayedCard);

    protected bool AllCardSameValue(List<CardData> cardList)
    {
        int firstValue = cardList[0].GetValue();
        for (var i = 1; i < cardList.Count; i++)
        {
            if (cardList[i].GetValue() != firstValue) return false;
        }
        return true;
    }

    protected bool AllCardSameKind(List<CardData> cardList)
    {
        int firstKind = cardList[0].GetKind();
        for (var i = 1; i < cardList.Count; i++)
        {
            if (cardList[i].GetKind() != firstKind) return false;
        }
        return true;
    }
}

public class SingleCombination : RuleCardCombination
{
    public override int GetCardCount()
    {
        return 1;
    }

    public override bool IsNewHigherRank(PlayedCardCombination prevPlayedCard, PlayedCardCombination newPlayedCard)
    {
        return prevPlayedCard.cardList[0] < newPlayedCard.cardList[0];
    }
}

public class PairsCombination : RuleCardCombination
{
    public override int GetCardCount()
    {
        return 2;
    }

    public override CombinationValidationResult IsValidCombination(PlayedCardCombination playedCard)
    {
        var baseResult = base.IsValidCombination(playedCard);
        baseResult.isValid = baseResult.isValid && AllCardSameValue(playedCard.GetCardDataList());
        return baseResult;
    }

    public override bool IsNewHigherRank(PlayedCardCombination prevPlayedCard, PlayedCardCombination newPlayedCard)
    {
        int highestValue = 0;
        foreach (var card in prevPlayedCard.GetCardDataList())
        {
            if (card.generalValue > highestValue) highestValue = card.generalValue;
        }
        foreach (var card in newPlayedCard.GetCardDataList())
        {
            if (card.generalValue > highestValue) return true;
        }
        return false;
    }
}

public class TriplesCombination : RuleCardCombination
{
    public override int GetCardCount()
    {
        return 3;
    }

    public override CombinationValidationResult IsValidCombination(PlayedCardCombination playedCard)
    {
        var baseResult = base.IsValidCombination(playedCard);
        baseResult.isValid = baseResult.isValid && AllCardSameValue(playedCard.GetCardDataList());
        return baseResult;
    }

    public override bool IsNewHigherRank(PlayedCardCombination prevPlayedCard, PlayedCardCombination newPlayedCard)
    {
        int highestValue = 0;
        foreach (var card in prevPlayedCard.GetCardDataList())
        {
            if (card.generalValue > highestValue) highestValue = card.generalValue;
        }
        foreach (var card in newPlayedCard.GetCardDataList())
        {
            if (card.generalValue > highestValue) return true;
        }
        return false;
    }
}

public class FiveCombination : RuleCardCombination
{

    public override int GetCardCount()
    {
        return 5;
    }

    public override bool IsNewHigherRank(PlayedCardCombination prevPlayedCard, PlayedCardCombination newPlayedCard)
    {
        if (prevPlayedCard.combinationEnum != newPlayedCard.combinationEnum)
        {
            return prevPlayedCard.combinationEnum < newPlayedCard.combinationEnum;
        }
        else if (prevPlayedCard.combinationEnum != FiveCombinationEnum.none)
        {
            return prevPlayedCard.customComparisonValue < newPlayedCard.customComparisonValue;
        }
        int highestValue = 0;
        foreach (var card in prevPlayedCard.GetCardDataList())
        {
            if (card.generalValue > highestValue) highestValue = card.generalValue;
        }
        foreach (var card in newPlayedCard.GetCardDataList())
        {
            if (card.generalValue > highestValue) return true;
        }
        return false;
    }

    public override CombinationValidationResult IsValidCombination(PlayedCardCombination playedCard)
    {
        var baseResult = base.IsValidCombination(playedCard);
        if (!baseResult.isValid)
        {
            return baseResult;
        }
        if (AllCardSameKind(playedCard.GetCardDataList()))
        {
            var cardDataList = playedCard.GetCardDataList().OrderBy(x => x.faceValue).ToList();
            var prevFaceValue = cardDataList[0].faceValue;
            var highestValue = cardDataList[0].generalValue;
            for (var i = 1; i < cardDataList.Count; i++)
            {
                prevFaceValue++;
                if (prevFaceValue != cardDataList[i].faceValue)
                {
                    baseResult.fiveCombinationEnum = FiveCombinationEnum.flush;
                    baseResult.comparisonValue = cardDataList[i].GetKind();
                    return baseResult;
                }
                if (highestValue < cardDataList[i].generalValue)
                {
                    highestValue = cardDataList[i].generalValue;
                }
            }
            baseResult.fiveCombinationEnum = FiveCombinationEnum.straightFlush;
            baseResult.comparisonValue = highestValue;
            return baseResult;
        }
        else
        {
            var cardDataList = playedCard.GetCardDataList().OrderBy(x => x.faceValue).ToList();
            Dictionary<int, int> allFaceValue = new Dictionary<int, int>();
            allFaceValue.Add(cardDataList[0].faceValue, 1);
            var prevFaceValue = cardDataList[0].faceValue;
            bool isStraight = true;
            var highestValue = cardDataList[0].generalValue;
            for (var i = 1; i < cardDataList.Count; i++)
            {
                if (allFaceValue.ContainsKey(cardDataList[i].faceValue))
                {
                    allFaceValue[cardDataList[i].faceValue]++;
                    if (allFaceValue[cardDataList[i].faceValue] == 4)
                    {
                        baseResult.fiveCombinationEnum = FiveCombinationEnum.fourOfAKind;
                        baseResult.comparisonValue = cardDataList[i].faceValue;
                        return baseResult;
                    }
                }
                else
                {
                    allFaceValue.Add(cardDataList[i].faceValue, 1);
                }
                prevFaceValue++;
                if (prevFaceValue != cardDataList[i].faceValue)
                {
                    isStraight = false;
                }
                if (highestValue < cardDataList[i].generalValue)
                {
                    highestValue = cardDataList[i].generalValue;
                }
            }
            if (allFaceValue.Keys.Count == 2)
            {
                baseResult.fiveCombinationEnum = FiveCombinationEnum.fullHouse;
                foreach (var value in allFaceValue)
                {
                    if (value.Value == 3) baseResult.comparisonValue = value.Key;
                }
                return baseResult;
            }
            if (isStraight)
            {
                baseResult.fiveCombinationEnum = FiveCombinationEnum.straight;
                baseResult.comparisonValue = highestValue;
                return baseResult;
            }
        }
        baseResult.isValid = false;
        return baseResult;
    }
}

public enum FiveCombinationEnum
{
    none,
    straight,
    flush,
    fullHouse,
    fourOfAKind,
    straightFlush
}

public struct CombinationValidationResult
{
    public bool isValid;
    public FiveCombinationEnum fiveCombinationEnum;
    public int comparisonValue;
}

/*public class StraightFiveCombination : FiveCombination
{
    public override bool IsValidCombination(PlayedCardCombination playedCard)
    {
        if (base.IsValidCombination(playedCard) && AllCardSameKind(playedCard.GetCardDataList()))
        {
            var cardDataList = playedCard.GetCardDataList().OrderBy(x => x.generalValue).ToList();
            var prevFaceValue = cardDataList[0].faceValue;
            for (var i = 1; i < cardDataList.Count; i++)
            {
                prevFaceValue++;
                if (prevFaceValue != cardDataList[i].faceValue) return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
*/