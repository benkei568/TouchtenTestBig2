using System.Collections.Generic;
using UnityEngine;
using ObjectPooling;

namespace VFXSystem {

    public static class VFXObjectPoolExtension
    {
        public static void InitVFXPools(this ObjectPools pool)
        {
            Dictionary<VFXEnum, GameObject> vfxDict = VFXSystem.vfxDictionary;
            foreach (KeyValuePair<VFXEnum, GameObject> vfxPair in vfxDict)
            {
                pool.CreateNewPool(new ObjectPoolConfig()
                {
                    id = vfxPair.Key.ToString(),
                    initialCount = 1,
                    prefab = vfxPair.Value
                });
            }
        }

        public static GameObject ActivateObject(this ObjectPools pool, VFXEnum vfxId, Transform transform, Transform parent = null)
        {
            return ObjectPools.Instance.ActivateObject(vfxId.ToString(), parent);
        }
    }
}