using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

namespace VFXSystem {
    [System.Serializable]
    public class VFXPathDictionary : SerializableDictionaryBase<VFXEnum, string> { }

    public class VFXDatabase : ScriptableObject
    {
        public VFXPathDictionary vfxDictionary;
    }
}