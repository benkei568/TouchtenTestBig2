using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "CharacterSpriteDatabase", menuName = "CharacterSpriteDatabaseSO")]
public class CharacterSpriteDatabase : ScriptableObject
{
    public CharacterSpriteDictionary CharacterSpriteDictionary;

}

[System.Serializable]
public class CharacterSpriteDictionary : SerializableDictionaryBase<int, CharacterSpriteExpression> { }

[System.Serializable]
public struct CharacterSpriteExpression
{
    public Sprite normal;
    public Sprite happy;
    public Sprite angry;
}