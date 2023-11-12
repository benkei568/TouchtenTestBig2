using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "CardDictionarySO", menuName = "CardDictionarySO")]
public class CardDictionarySO : ScriptableObject
{
    public int kindCount;
    public int cardSetCount;
    [SerializeField]
    private CardDictionary cardDictionary;

    public List<int> GetAllKeyDictionary()
    {
        return cardDictionary.Keys.ToList();
    }

    public CardData GetCard(Vector2 vCard)
    {
        foreach (var keyCard in cardDictionary)
        {
            if (keyCard.Value.v2_value == vCard) return keyCard.Value;
        }
        return new CardData();
    }

    public CardData GetCardByValue(int generalValue)
    {
        foreach (var keyCard in cardDictionary)
        {
            if (keyCard.Value.generalValue == generalValue) return keyCard.Value;
        }
        return new CardData();
    }

    public CardData GetCardByID(int cardID)
    {
        return cardDictionary[cardID];
    }

}
[Serializable]
public class CardDictionary : SerializableDictionaryBase<int, CardData> { };

[Serializable]
public struct CardData
{
    public Sprite sprite;
    public Vector2 v2_value; // x -> num value (3 = 0, 4 = 1, A = 11, 2 = 12), y -> kind value (diamond = 0, spade = 3) 
    public int generalValue; // 3diamond = 0, 3clubs = 1, 3heart = 2
    public int faceValue;   // A = 1, 2 = 2, 3 = 3, J = 11, Q = 12, K = 13

    public int GetValue()
    {
        return (int)v2_value.x;
    }

    public int GetKind()
    {
        return (int)v2_value.y;
    }
}
