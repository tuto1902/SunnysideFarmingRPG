using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonoBehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    
    [SerializeField] private Pool[] pool = null;
    [SerializeField] private Transform objectPoolTransform = null;

    [System.Serializable]

    public struct Pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        for (int i = 0; i < pool.Length; i++)
        {
            CreatePool(pool[i].prefab, pool[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject($"{prefabName}Anchor");
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newGameObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newGameObject.SetActive(false);

                poolDictionary[poolKey].Enqueue(newGameObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject gameObjectToUse = GetObjectFromPool(poolKey);
            ResetObject(position, rotation, gameObjectToUse, prefab);

            return gameObjectToUse;
        }
        else
        {
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject gameObjectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(gameObjectToReuse);

        if (gameObjectToReuse.activeSelf == true)
        {
            gameObjectToReuse.SetActive(false);
        }

        return gameObjectToReuse;
    }

    private void ResetObject(Vector3 position, Quaternion rotation, GameObject gameObjectToUse, GameObject prefab)
    {
        gameObjectToUse.transform.position = position;
        gameObjectToUse.transform.rotation = rotation;
        gameObjectToUse.transform.localScale = prefab.transform.localScale;
    }
}
