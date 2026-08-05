using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum PoolID
{
    Rat, Bat, Slime, Zombie, Ghost
}

public class EnemyPoolManager : MonoBehaviour
{

  

    public static EnemyPoolManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [System.Serializable]
    public class Pool
    {
        public PoolID id;
        public GameObject prefab;
        public int size = 10;
        [HideInInspector]
        public List<GameObject> objects;
    }

    [SerializeField] public List<Pool> pools;

    private Dictionary<PoolID, Pool> _poolLookup;

    void Awake()
    {
        Instance = this;

        _poolLookup = new Dictionary<PoolID, Pool>();
        foreach(var pool in pools)
        {
            pool.objects = new List<GameObject>();

            for(int i = 0; i < pool.size; i++)
            {
                GameObject go = Instantiate(pool.prefab);
                go.SetActive(false);
                pool.objects.Add(go);
            }

            _poolLookup.Add(pool.id, pool);
        }
    }

    public GameObject Get(PoolID id)
    {
        if(!_poolLookup.TryGetValue(id, out Pool pool))
        {
            Debug.Log($"No pool for object {id} !");
            return null;
        }

        foreach(var obj in pool.objects)
        {
            if(!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        //pool is full
        return null;

    }

    public GameObject Get(PoolID id, Vector3 pos, Quaternion rot, Vector3? initialTarget = null)
    {
        GameObject obj = Get(id);
        if (obj == null) return null;

        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);
        if(obj.TryGetComponent<IEnemyAI>(out var enemy))
        {

            if (initialTarget.HasValue)
            {
                
                enemy.Tick(initialTarget.Value);
            }


        }
        return obj;
    }


    public Pool GetPool(PoolID id)
    {
        _poolLookup.TryGetValue(id, out var pool);
        return pool;
    }



}
