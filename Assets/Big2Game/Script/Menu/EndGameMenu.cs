using UnityEngine;
using TMPro;

public class EndGameMenu : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI title;

    private void Start()
    {
        EventManager.onEndGameEvent += OnEndGame;
    }

    private void OnDestroy()
    {
        EventManager.onEndGameEvent -= OnEndGame;
    }

    void OnEndGame(ParticipantScript winnerParticipant)
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        title.text = winnerParticipant.participantName + " Win";
    }

}