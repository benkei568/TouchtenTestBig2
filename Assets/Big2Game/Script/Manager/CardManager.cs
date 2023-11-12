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
            new SingleCombination(), new PairsCombination(), new TriplesCombination(), new StraightFiveCombination()
        };
    }

    public CardDictionarySO GetDictionarySO()
    {
        return cardDictionary;
    }

    public PlayedCardCombination ValidateCombination(PlayedCardCombination playedCard)
    {
        for (var i = 0; i < ruleCardCombinationsList.Count; i++)
        {
            if (ruleCardCombinationsList[i].IsValidCombination(playedCard))
            {
                playedCard.combinationID = i;
                return playedCard;
            }
        }
        playedCard.combinationID = -1;
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

    public PlayedCardCombination()
    {
        combinationID = -1;
        cardList = new List < int>();
    }

    public bool IsCombinationValid()
    {
        return combinationID >= 0;
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
    public abstract int GetCardCount();

    public virtual bool IsValidCombination(PlayedCardCombination playedCard)
    {
        return playedCard.cardList.Count == GetCardCount();
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

    public override bool IsValidCombination(PlayedCardCombination playedCard)
    {
        return base.IsValidCombination(playedCard) && AllCardSameValue(playedCard.GetCardDataList());
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

    public override bool IsValidCombination(PlayedCardCombination playedCard)
    {
        return base.IsValidCombination(playedCard) && AllCardSameValue(playedCard.GetCardDataList());
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

public class StraightFiveCombination : FiveCombination
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