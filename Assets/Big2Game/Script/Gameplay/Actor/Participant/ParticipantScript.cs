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
    public bool currentActiveParticipant;

    ParticipantBaseState currentState;
    protected ParticipantReadyState readyState;
    protected ParticipantWaitingState waitingState;
    protected ParticipantPlayingState playingState;

    protected List<int> currentCard = new();
    protected PlayedCardCombination prevSubmitCard = new();
    List<CardScript> displaySubmitedCardScripts = new();

    public int participantID;

    protected virtual void Awake()
    {
        SetActiveParticipant(false);
    }

    protected virtual void Start()
    {
        SetState(readyState);
    }

    public void SetParticipantID(int newID)
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
        SetState(waitingState);
        SetActiveParticipant( false);
    }

    void SetActiveParticipant(bool isActive)
    {
        currentActiveParticipant = isActive;
        uIStats.SetParticipantActiveTurn(isActive);
    }

    public void PassTurn()
    {
        GameplayManager.instance.PassTurn(participantID);
    }

    public PlayedCardCombination GetPrevSubmitCard()
    {
        return prevSubmitCard;
    }

    public void ParticipantSubmit(PlayedCardCombination playedCard)
    {
        prevSubmitCard = playedCard;
        foreach (var card in playedCard.cardList)
        {
            currentCard.Remove(card);
        }
        OnCardCountChanged(currentCard.Count);
        OnSubmitCardChange(prevSubmitCard);
        GameplayManager.instance.SubmitCardCombination(participantID, playedCard, currentCard.Count <= 0);
    }

    void SetState(ParticipantBaseState newState)
    {
        if (currentState != null) currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }
}

