using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class CardScript : MonoBehaviour
{
    public Image cardImage;
    public Button button;
    public int cardID { get; private set; }
    CardData cardData;

    public void Instantiate(CardData newData, int newCardID, ReactiveCommand<(int, CardScript)> buttonCommand = null)
    {
        cardData = newData;
        cardID = newCardID;
        cardImage.sprite = cardData.sprite;
        if (buttonCommand != null)
        {
            void OnCardButtonClicked(Unit obj)
            {
                buttonCommand.Execute((cardID, this));
            }

            button.OnClickAsObservable().Subscribe(OnCardButtonClicked);
            
        }
    }

    public void SetButtonInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }


}