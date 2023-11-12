using UnityEngine;

public  class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public delegate void EndGame(ParticipantScript winnerParticipant);
    public static event EndGame onEndGameEvent;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void OnEndGame(ParticipantScript winnerParticipant)
    {
        onEndGameEvent?.Invoke(winnerParticipant);
    }
}