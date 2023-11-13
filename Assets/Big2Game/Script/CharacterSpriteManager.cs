using UnityEngine;

public class CharacterSpriteManager : MonoBehaviour
{
    public CharacterSpriteDatabase spriteDatabase;

    public CharacterSpriteExpression GetSprite(int id)
    {
        return spriteDatabase.CharacterSpriteDictionary[id];
    }

    public int Length()
    {
        return spriteDatabase.CharacterSpriteDictionary.Count;
    }
}
