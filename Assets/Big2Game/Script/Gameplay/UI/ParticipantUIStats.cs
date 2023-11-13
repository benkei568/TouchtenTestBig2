using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ParticipantUIStats : MonoBehaviour
{
    public Image characterImage;
    public TextMeshProUGUI participantName;
    public TextMeshProUGUI participantCardOwned;
    public GameObject activeParticipantImage;

    int characterID;
    Coroutine currentCoroutine;


    public void UpdateName(string newName)
    {
        participantName.text = newName;
    }

    public void UpdateCharacterSprite(int newCharacterID)
    {
        characterID = newCharacterID;
        characterImage.sprite = GameManager.instance.spriteManager.GetSprite(characterID).normal;
    }

    public void OnParticipantSubmit()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(SetExpressionSprite(GameManager.instance.spriteManager.GetSprite(characterID).happy));
    }

    public void OnParticipantPass()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(SetExpressionSprite(GameManager.instance.spriteManager.GetSprite(characterID).angry));
    }

    public void UpdateCardOwned(int newQuantity)
    {
        participantCardOwned.text = "Card Left : " + newQuantity;
    }

    public void SetParticipantActiveTurn(bool isActive)
    {
        activeParticipantImage.SetActive(isActive);
    }

    public IEnumerator SetExpressionSprite(Sprite newExpression)
    {
        characterImage.sprite = newExpression;
        yield return new WaitForSeconds(3);
        characterImage.sprite = GameManager.instance.spriteManager.GetSprite(characterID).normal;
    }

}