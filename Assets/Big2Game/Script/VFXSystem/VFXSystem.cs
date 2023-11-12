using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFXSystem
{
    public class VFXSystem : MonoBehaviour
    {
        public static Dictionary<VFXEnum, GameObject> vfxDictionary { get; private set; }

        private static VFXDatabase vfxDB;
        
        public delegate void VFXLoadFinish();
        public static event VFXLoadFinish onVFXLoadFinished;

        public static IEnumerator LoadAllVFX()
        {
            vfxDictionary = new Dictionary<VFXEnum, GameObject>();
            vfxDB = Resources.Load<VFXDatabase>("VFXDB");

            var dictKeys = vfxDB.vfxDictionary.Keys;
            foreach (var key in dictKeys)
            {
                ResourceRequest req = Resources.LoadAsync<GameObject>(vfxDB.vfxDictionary[key]);
                yield return req;
                GameObject loadedVFX = req.asset as GameObject;
                if(loadedVFX != null)
                {
                    vfxDictionary.Add(key, loadedVFX);
                }
                else
                {
                    Debug.LogError("error loading vfx: " + vfxDB.vfxDictionary[key]);
                }
            }
            onVFXLoadFinished?.Invoke();
            onVFXLoadFinished = null;
        }

        public static GameObject GetVFX(VFXEnum id)
        {
            GameObject retval;
            if(vfxDictionary.TryGetValue(id, out retval))
            {
                return retval;
            }
            else
            {
                return null;
            }
        }
    }
}