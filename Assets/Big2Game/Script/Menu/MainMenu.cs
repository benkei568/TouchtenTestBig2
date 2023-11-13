using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image characterImage;

    public void RightButton()
    {
        GameManager.instance.ChangeCharacterSprite(true);
        UpdateImage();
    }

    public void LeftButton()
    {
        GameManager.instance.ChangeCharacterSprite(false);
        UpdateImage();
    }

    void UpdateImage()
    {
        characterImage.sprite = GameManager.instance.GetCurrentSprite();
    }

    public void StartGame()
    {
        GameManager.instance.LoadGameScene();
    }
}