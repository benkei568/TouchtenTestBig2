using System.Linq;
using UnityEngine;
using UniRx;
using System.Collections.Generic;
using ObjectPooling;

public abstract class ParticipantScript : MonoBehaviour
{
    public ParticipantUIStats uIStats;
    public Transform submitedCardParent;
    public Transform submitedCardTransform;

    [Header("RuntimeVariable")]
    public bool isCurrentActive;

    ParticipantBaseState currentState;
    protected ParticipantReadyState readyState;
    protected ParticipantWaitingState waitingState;
    protected ParticipantPlayingState playingState;

    protected List<int> currentCard = new();
    protected PlayedCardCombination prevSubmitCard = new();
    List<CardScript> displaySubmitedCardScripts = new();

    public int participantID;
    public string participantName;
    public int spriteID;

    protected virtual void Awake()
    {
       
    }

    protected virtual void Start()
    {
        SetActiveParticipant(false);
        SetState(readyState);
    }

    public virtual void SetParticipantID(int newID)
    {
        participantID = newID;
    }

    public void GameplayUpdate(GameplayManager gameplay, float deltaTime)
    {
        currentState.UpdateState(this, gameplay, deltaTime);
    }

    public void SetCardList(List<int> newCardList)
    {
        currentCard = newCardList.OrderBy(x => x).ToList(); 
        OnCardCountChanged(currentCard.Count);
    }

    protected virtual void OnCardCountChanged(int newCardCount)
    {
        uIStats.UpdateCardOwned(newCardCount);
    }

    void OnSubmitCardChange(PlayedCardCombination newCardCombination)
    {
        for (var i = 0; i < newCardCombination.cardList.Count; i++)
        {
            if (i >= displaySubmitedCardScripts.Count)
            {
                CardScript newCard = ObjectPools.Instance.ActivateObject(CardManager.instance.cardPrefabID, submitedCardTransform, submitedCardParent).GetComponent<CardScript>();
                displaySubmitedCardScripts.Add(newCard);
            }
            displaySubmitedCardScripts[i].Instantiate(CardManager.instance.GetDictionarySO().GetCardByID(newCardCombination.cardList[i]), newCardCombination.cardList[i]);
        }
        while (displaySubmitedCardScripts.Count > newCardCombination.cardList.Count)
        {
            ObjectPools.Instance.DeactivateObject(displaySubmitedCardScripts[displaySubmitedCardScripts.Count - 1].gameObject);
            displaySubmitedCardScripts.RemoveAt(displaySubmitedCardScripts.Count - 1);
        }
    }

    public void StartTurn()
    {
        SetState(playingState);
        prevSubmitCard = new();
        OnSubmitCardChange(prevSubmitCard);
        SetActiveParticipant(true);
    }

    public void EndTurn()
    {
        SetActiveParticipant(false);
        SetState(waitingState);
    }

    protected virtual void SetActiveParticipant(bool isActive)
    {
        isCurrentActive = isActive;
        uIStats.SetParticipantActiveTurn(isActive);
    }

    public void PassTurn()
    {
        GameplayManager.instance.PassTurn(participantID);
        uIStats.OnParticipantPass();
    }

    public PlayedCardCombination GetPrevSubmitCard()
    {
        return prevSubmitCard;
    }

    public void ParticipantSubmit(PlayedCardCombination playedCard)
    {
        prevSubmitCard = new PlayedCardCombination() { combinationID = playedCard.combinationID, cardList = new List<int>(playedCard.cardList) };
        foreach (var card in prevSubmitCard.cardList)
        {
            currentCard.Remove(card);
        }
        OnCardCountChanged(currentCard.Count);
        OnSubmitCardChange(prevSubmitCard);
        GameplayManager.instance.SubmitCardCombination(participantID, prevSubmitCard, currentCard.Count <= 0);
        uIStats.OnParticipantSubmit();
    }

    void SetState(ParticipantBaseState newState)
    {
        if (currentState != null) currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

}

