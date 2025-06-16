using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("PoolManager");
                instance = obj.GetComponent<PoolManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private Dictionary<string, ObjectPool> pools = new();

    public void CreatePool(GameObject prefab, int initialSize)
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new ObjectPool(prefab, initialSize, transform));
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            CreatePool(prefab, 10);
        }
        return pools[key].GetObject();
    }

    public void ReturnObject(GameObject obj)
    {
        string key = obj.name.Replace("(Clone)", "");
        if (pools.ContainsKey(key))
        {
            pools[key].ReturnObject(obj);
        }
    }
}