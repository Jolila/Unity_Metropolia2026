using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum EnemyType
{
    RatLeader, RatFollower, SlimeLeader, SlimeFollower, ZombieLeader, ZombieFollower, Bat, Ghost
}

public class EnemyPoolManager : MonoBehaviour
{



    public static EnemyPoolManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [System.Serializable]
    public class Pool
    {
        public EnemyType type;
        public GameObject prefab;
        public int size = 10;
        [HideInInspector]
        public List<GameObject> objects;
    }

    [SerializeField] public List<Pool> pools;

    private Dictionary<EnemyType, Pool> _poolLookup;

    void Awake()
    {
        Instance = this;

        _poolLookup = new Dictionary<EnemyType, Pool>();
        foreach(var pool in pools)
        {
            pool.objects = new List<GameObject>();

            for(int i = 0; i < pool.size; i++)
            {
                GameObject go = Instantiate(pool.prefab);
                go.SetActive(false);
                pool.objects.Add(go);
            }

            _poolLookup.Add(pool.type, pool);
        }
    }

    public GameObject Get(EnemyType type)
    {
        if(!_poolLookup.TryGetValue(type, out Pool pool))
        {
            Debug.Log($"No pool for object {type} !");
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

    public GameObject Get(EnemyType type, Vector3 pos, Quaternion rot, Vector3? initialTarget = null)
    {
        GameObject obj = Get(type);
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


    public Pool GetPool(EnemyType type)
    {
        _poolLookup.TryGetValue(type, out var pool);
        return pool;
    }



}
