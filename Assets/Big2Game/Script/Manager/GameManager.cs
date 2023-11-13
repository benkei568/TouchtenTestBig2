using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CharacterSpriteManager spriteManager;

    public int characterSprite = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            spriteManager = GetComponent<CharacterSpriteManager>();
        }
    }

    public Sprite GetCurrentSprite()
    {
        return spriteManager.GetSprite(characterSprite).normal;
    }

    public void ChangeCharacterSprite(bool right = true)
    {
        if (right)
            characterSprite++;
        else
            characterSprite--;
        if (characterSprite >= spriteManager.Length())
        {
            characterSprite = 0;
        }
        else if (characterSprite < 0)
        {
            characterSprite = spriteManager.Length() - 1;
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(Big2.gameplayScene);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Big2.menuScene);
    }

}