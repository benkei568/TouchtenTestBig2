using UnityEngine;
using TMPro;
using UniRx;

public class ParticipantUIStats : MonoBehaviour
{
    public TextMeshProUGUI participantName;
    public TextMeshProUGUI participantCardOwned;
    public GameObject activeParticipantImage;

    public void UpdateCardOwned(int newQuantity)
    {
        participantCardOwned.text = "Card Left : " + newQuantity;
    }

    public void SetParticipantActiveTurn(bool isActive)
    {
        activeParticipantImage.SetActive(isActive);
    }

}