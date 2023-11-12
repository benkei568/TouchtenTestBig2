using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public class ObjectPools : MonoBehaviour
    {
        public static ObjectPools Instance { get; private set; }

        public bool initializeOnStart = true;
        public List<ObjectPoolConfig> initialPools;

        private Dictionary<string, RuntimeObjectPool> runtimePools;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (initializeOnStart)
                InitializePools();
        }

        public void InitializePools()
        {
            if (runtimePools != null) DestroyPools();
            runtimePools = new Dictionary<string, RuntimeObjectPool>();

            for (int i = 0; i < initialPools.Count; i++)
            {
                CreateNewPool(initialPools[i]);
            }
        }

        private void DestroyPools()
        {
            foreach (KeyValuePair<string, RuntimeObjectPool> pool in runtimePools)
            {
                for (int i = 0; i < pool.Value.pooledObject.Count; i++)
                {
                    Destroy(pool.Value.pooledObject[i]);
                }
                pool.Value.pooledObject.Clear();
            }
        }

        public void CreateNewPool(ObjectPoolConfig config)
        {
            RuntimeObjectPool newPool = new RuntimeObjectPool(config.prefab); 
            for (int i = 0; i < config.initialCount; i++)
            {
                GameObject newObject = Instantiate(newPool.prefab, transform);
                newObject.SetActive(false);
                newPool.pooledObject.Add(newObject);
            }
            runtimePools.Add(config.id, newPool);
        }

        public GameObject ActivateObject(string id, Transform objTransform, Transform parent = null)
        {
            RuntimeObjectPool pool;
            if(runtimePools.TryGetValue(id, out pool))
            {
                for (int i = 0; i < pool.pooledObject.Count; i++)
                {
                    if (!pool.pooledObject[i].activeInHierarchy)
                    {
                        pool.pooledObject[i].transform.SetParent(parent != null ? parent : transform); //Debug.Log("scale 2" + pool.pooledObject[i].transform.localScale);
                        pool.pooledObject[i].transform.position = objTransform.position;
                        pool.pooledObject[i].transform.rotation = objTransform.rotation;
                        pool.pooledObject[i].transform.localScale = objTransform.localScale; //Debug.Log("scale " + pool.pooledObject[i].transform.localScale);
                        pool.pooledObject[i].SetActive(true);
                        return pool.pooledObject[i];
                    }
                }
                GameObject newObject = Instantiate(pool.prefab, parent != null ? parent : transform);
                newObject.transform.position = objTransform.position;
                newObject.transform.rotation = objTransform.rotation;
                newObject.transform.localScale = objTransform.localScale;
                pool.pooledObject.Add(newObject);
                return newObject;
            }
            else
            {
                Debug.LogWarning("object pool not found " + id);
                return null;
            }
        }

        public void DeactivateObject(GameObject deactivatedObject)
        {
            if(deactivatedObject.transform.parent != transform)
            {
                deactivatedObject.transform.SetParent(transform);
            }
            deactivatedObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class ObjectPoolConfig
    {
        public string id;
        public GameObject prefab;
        public int initialCount;
    }

    public class RuntimeObjectPool
    {
        public GameObject prefab;
        public List<GameObject> pooledObject;

        public RuntimeObjectPool(GameObject _prefab)
        {
            prefab = _prefab;
            pooledObject = new List<GameObject>();
        }
    }
}