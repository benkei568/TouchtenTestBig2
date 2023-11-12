using UnityEngine;
using System.Collections.Generic;

public class GameReadyState : GameBaseState
{
    public override void EnterState(GameplayManager gameplay)
    {
        gameplay.Instantiate();
    }

    public override void ExitState(GameplayManager gameplay)
    {

    }

    public override void UpdateState(GameplayManager gameplay, float deltaTime)
    {
        StartGiveCards(gameplay);
        gameplay.EndReadyState();
    }

    void StartGiveCards(GameplayManager gameplay)
    {
        List<int> cardIDList = CardManager.instance.GetDictionarySO().GetAllKeyDictionary();
        for (int i = 0; i < cardIDList.Count; i++)
        {
            int temp = cardIDList[i];

            int randomIndex = Random.Range(i, cardIDList.Count);
            cardIDList[i] = cardIDList[randomIndex];
            cardIDList[randomIndex] = temp;
        }
        int splitSize = cardIDList.Count / gameplay.participantList.Count;
        var splittedCardList = GeneralUtilities.ChunkBy(cardIDList, splitSize);
        for (var i = 0; i < gameplay.participantList.Count; i++)
        {
            gameplay.participantList[i].SetCardList(splittedCardList[i]);
            if (splittedCardList[i].Contains(0))
            {
                gameplay.CurrentActiveParticipant = i;
            }
               
        }
    }

    
}