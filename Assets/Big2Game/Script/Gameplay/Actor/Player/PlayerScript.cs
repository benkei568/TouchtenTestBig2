using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using ObjectPooling;

public class PlayerScript : ParticipantScript
{
    [Header("Initial Variable")]
    public Transform displayCardParent;
    public Transform displayCardTransform;

    public Button SubmitButton;
    public Button PassButton;
    public Button ClearButton;

    public List<CardScript> currentDisplayCard = new();
    PlayedCardCombination currentChoosenCombination = new();
    List<CardScript> currentChoosenCard = new();

    ReactiveCommand<(int, CardScript)> ChooseCardCommand { get; } = new();
    ReactiveCommand SkipCommand { get; } = new();
    ReactiveCommand SubmitCommand { get; } = new();
    ReactiveCommand ClearCommand { get; } = new();

    protected override void Awake()
    {
        base.Awake();
        readyState = new ParticipantReadyState();
        playingState = new PlayerPlayingState();
        waitingState = new ParticipantWaitingState();
        currentChoosenCombination = new PlayedCardCombination();
        ChooseCardCommand.Subscribe(tuple =>
        {
            if (currentChoosenCombination.cardList.Contains(tuple.Item1))
            {
                ReturnCard(tuple.Item1, tuple.Item2);
                SortCard(currentDisplayCard);
                OnUpdateChoosenCombination();
            }
            else if (currentChoosenCard.Count < 5)
            {
                ChooseCard(tuple.Item1, tuple.Item2);
                SortCard(currentChoosenCard);
                OnUpdateChoosenCombination();
            }
        }).AddTo(this);
        SkipCommand.Subscribe( _ =>
        {
            ClearCommand.Execute();
            PassTurn();
        }).AddTo(this);
        SubmitCommand.Subscribe(_ =>
        {
            ParticipantSubmit(currentChoosenCombination);
            while (currentChoosenCard.Count > 0)
            {
                currentChoosenCard[0].SetButtonInteractable(false);
                ObjectPools.Instance.DeactivateObject(currentChoosenCard[0].gameObject);
                currentChoosenCombination.cardList.Remove(currentChoosenCard[0].cardID);
                currentChoosenCard.RemoveAt(0);
            }
            OnUpdateChoosenCombination();
        }).AddTo(this);
        ClearCommand.Subscribe(_ =>
        {
            while (currentChoosenCard.Count > 0)
            {
                ReturnCard(currentChoosenCard[0].cardID, currentChoosenCard[0]);
            }
            OnUpdateChoosenCombination();
            SortCard(currentDisplayCard);
        }).AddTo(this);
        PassButton.OnClickAsObservable().Subscribe(_ => SkipCommand.Execute());
        SubmitButton.OnClickAsObservable().Subscribe(_ => SubmitCommand.Execute());
        ClearButton.OnClickAsObservable().Subscribe(_ => ClearCommand.Execute());
    }

    protected override void Start()
    {
        base.Start();
        uIStats.UpdateCharacterSprite(GameManager.instance.characterSprite);
        /*SkipCommand.CanExecute
            .Where(_ => !GameplayManager.instance.MustSubmitTurn() && currentActiveParticipant)
            .SubscribeToInteractable(PassButton);
        SubmitCommand.CanExecute
            .Where(_ =>
            {
                if (currentActiveParticipant && currentChoosenCombination.IsCombinationValid())
                {
                    var prevCombination = GameplayManager.instance.GetLastActiveCombination();
                    if (prevCombination.IsCombinationValid())
                        return CardManager.instance.IsCombinationHigherValue(prevCombination, currentChoosenCombination);
                    else
                        return true;
                }
                return false;
            })
            .SubscribeToInteractable(SubmitButton);
        ClearCommand.CanExecute
             .Where(_ => currentChoosenCard.Count > 0 && currentActiveParticipant)
             .SubscribeToInteractable(ClearButton);*/
    }

    public override void SetParticipantID(int newID)
    {
        base.SetParticipantID(newID);
        participantName = "Player";
        uIStats.UpdateName(participantName);
    }

    protected override void OnCardCountChanged(int newCardCount)
    {
        base.OnCardCountChanged(newCardCount);
        OnCurrentCardChange();
    }

    protected override void SetActiveParticipant(bool isActive)
    {
        base.SetActiveParticipant(isActive);
        UpdateAllButton();
        foreach (var card in currentDisplayCard)
        {
            card.SetButtonInteractable(isActive);
        }
    }

    void OnCurrentCardChange()
    {
        for (var i = 0; i < currentCard.Count; i++)
        {
            if (i >= currentDisplayCard.Count)
            {
                CardScript newCard = ObjectPools.Instance.ActivateObject(CardManager.instance.cardPrefabID, displayCardTransform, displayCardParent).GetComponent<CardScript>();
                currentDisplayCard.Add(newCard);
            }
            currentDisplayCard[i].Instantiate(CardManager.instance.GetDictionarySO().GetCardByID(currentCard[i]), currentCard[i], ChooseCardCommand);
        }
        while (currentDisplayCard.Count > currentCard.Count)
        {
            ObjectPools.Instance.DeactivateObject(currentDisplayCard[currentDisplayCard.Count - 1].gameObject);
            currentDisplayCard.RemoveAt(currentDisplayCard.Count - 1);
        }
    }

    void ReturnCard(int cardID, CardScript cardScript)
    {
        currentChoosenCombination.cardList.Remove(cardID);
        currentChoosenCard.Remove(cardScript);
        currentDisplayCard.Add(cardScript);
        cardScript.transform.SetParent(displayCardParent);
    }

    void ChooseCard(int cardID, CardScript cardScript)
    {
        currentDisplayCard.Remove(cardScript);
        currentChoosenCombination.cardList.Add(cardID);
        currentChoosenCard.Add(cardScript);
        cardScript.transform.SetParent(submitedCardParent);
    }

    List<CardScript> SortCard(List<CardScript> cardList)
    {
        cardList = cardList.OrderBy(card => card.cardID).ToList();
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].transform.SetSiblingIndex(i);
        }
        return cardList;
    }

    void OnUpdateChoosenCombination()
    {
        currentChoosenCombination = CardManager.instance.ValidateCombination(currentChoosenCombination);
        UpdateSubmitButton();
        UpdateClearButton();
    }

    void UpdateAllButton()
    {
        UpdatePassButton();
        UpdateSubmitButton();
        UpdateClearButton();
    }

    void UpdatePassButton()
    {
        PassButton.interactable = !GameplayManager.instance.MustSubmitTurn() && isCurrentActive;
    } 

    void UpdateSubmitButton()
    {
        if (isCurrentActive && currentChoosenCombination.IsCombinationValid())
        {
            if (GameplayManager.instance.initialSubmit)
            {
                SubmitButton.interactable = currentChoosenCombination.InitialCombination();
            }
            else
            {
                var prevCombination = GameplayManager.instance.GetLastActiveCombination();
                if (prevCombination.IsCombinationValid())
                    SubmitButton.interactable = CardManager.instance.IsCombinationHigherValue(prevCombination, currentChoosenCombination);
                else
                    SubmitButton.interactable = true;
            }
        }
        else
        {
            SubmitButton.interactable = false;
        }
    }

    void UpdateClearButton()
    {
        Debug.Log("UpdateClearButton" + currentChoosenCard.Count);
        ClearButton.interactable = currentChoosenCard.Count > 0 && isCurrentActive;
    }
}