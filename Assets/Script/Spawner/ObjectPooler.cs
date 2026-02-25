using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;     
        public GameObject prefab; 
        public int size;   
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Singleton
    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // T?t ?i ch? d¨´ng
                obj.transform.SetParent(this.transform);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool v?i tag " + tag + " kh?ng t?n t?i!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // G?i h¨¤m OnEnable ho?c Reset tr?ng th¨¢i n?u c?n (Quan tr?ng)
        // IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        // if (pooledObj != null) pooledObj.OnObjectSpawn();

        // X?p l?i v¨¤o cu?i h¨¤ng ??i ?? t¨¢i s? d?ng sau n¨¤y
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}