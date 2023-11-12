using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace AudioSystem {
    [System.Serializable]
    public class AudioPathDictionary : SerializableDictionaryBase<AudioEventEnum, string> { }

    public class AudioDatabase : ScriptableObject
    {
        public AudioPathDictionary audioPathDictionary;
    }
}