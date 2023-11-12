using UnityEngine;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager instance;
    ReactiveProperty<GameBaseState> currentState = new ReactiveProperty<GameBaseState>();
    GameReadyState readyState = new GameReadyState();
    GameRunState runState = new GameRunState();
    GameEndState endState = new GameEndState();

    public List<ParticipantScript> participantList;
    int currentActiveParticipant = -1;
    public int CurrentActiveParticipant { get {return currentActiveParticipant; } 
        set 
        {
            if (currentActiveParticipant >= 0) participantList[currentActiveParticipant].EndTurn();
            currentActiveParticipant = value;
            if (lastSubmitedCardsParticipant == currentActiveParticipant) lastSubmitedCardsParticipant = -1;
            participantList[currentActiveParticipant].StartTurn();
        } }

    int lastActiveParticipant;
    int lastSubmitedCardsParticipant;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        this.UpdateAsObservable()
               .Subscribe(_ => currentState.Value.UpdateState(this, Time.deltaTime))
               .AddTo(this);
        Instantiate();
        for (var i = 0; i < participantList.Count; i++)
        {
            participantList[i].SetParticipantID(i);
        }
       
    }

    private void Start()
    {
        SetState(readyState);
    }

    public void Instantiate()
    {
        lastActiveParticipant = -1;
        lastSubmitedCardsParticipant = -1;
    }

    public void EndReadyState()
    {
        SetState(runState);
    }

    public void PassTurn(int participantID)
    {
        NextTurn();
    }

    public void SubmitCardCombination(int participantID, PlayedCardCombination playedCard, bool outOfCard)
    {
        lastSubmitedCardsParticipant = participantID;
        if (outOfCard) EndGame();
        else NextTurn();
    }

    void NextTurn()
    {
        if (CurrentActiveParticipant == participantList.Count - 1)
        {
            CurrentActiveParticipant = 0;
        }
        else
        {
            CurrentActiveParticipant++;
        }
    }

    void EndGame()
    {

    }

    public bool MustSubmitTurn()
    {
        return lastSubmitedCardsParticipant < 0;
    }

    public PlayedCardCombination GetLastActiveCombination()
    {
        if (lastSubmitedCardsParticipant >= 0 && lastSubmitedCardsParticipant != CurrentActiveParticipant)
        {
            return participantList[lastSubmitedCardsParticipant].GetPrevSubmitCard();
        }
        else
        {
            return new();
        }
    }


    void SetState(GameBaseState newState)
    {
        if (currentState.Value != null) currentState.Value.ExitState(this);
        currentState.Value = newState;
        currentState.Value.EnterState(this);
    }
}

