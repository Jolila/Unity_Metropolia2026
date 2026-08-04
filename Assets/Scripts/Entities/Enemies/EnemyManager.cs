using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{

    int currentPool = 0;
    Vector3 cachedPlayerPosition;
    private bool needsRetarget;
    int updatedPools = 0;
    [SerializeField] float retargetDistance = 2.5f;
    [SerializeField] EnemySpawner _spawner;
    [SerializeField] PoolManager _poolManager;

    Transform player;


    private int framesPerPoolUpdate = 2;

    private int frameCounter;
    bool gameStarted;


    [SerializeField] float interval = 0.000001f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        needsRetarget = false;
        gameStarted = false;
    }

    public void OnStartGame()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cachedPlayerPosition = player.position;
        needsRetarget = true;
        gameStarted = true;
    }

    public void spawnInitialEnemy()
    {
        _spawner.SpawnNewEnemy(null);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameStarted) return;

        //Debug.Log(GameManager.Instance.GetIsCountDown());
        if (GameManager.Instance.GetIsCountDown()) return;


        frameCounter++;
        if (frameCounter < framesPerPoolUpdate) return;

        frameCounter = 0;

       _spawner.SpawnNewEnemy(cachedPlayerPosition);
            

        if (!needsRetarget &&
    (player.position - cachedPlayerPosition).sqrMagnitude >
    retargetDistance * retargetDistance)
        {
            cachedPlayerPosition = player.position;
            needsRetarget = true;
            updatedPools = 0;
        }

        UpdatePool();
        ++currentPool;

        if (currentPool >= PoolManager.Instance.pools.Count)
            currentPool = 0;
        
    }


    void UpdatePool()
    {
        var pool = PoolManager.Instance.pools[currentPool];

        foreach(var obj in pool.objects)
        {
            if (!obj.activeInHierarchy) continue;

            IEnemyAI enemy = obj.GetComponent<IEnemyAI>();

            if (enemy == null) continue;
            enemy.Tick(cachedPlayerPosition);

            if (needsRetarget)
                enemy.UpdateTarget(cachedPlayerPosition);

           
        }

        if (needsRetarget)
        {
            updatedPools++;

            if (updatedPools >= PoolManager.Instance.pools.Count)
            {
                needsRetarget = false;
                updatedPools = 0;
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(cachedPlayerPosition, retargetDistance);
     


    }


}
