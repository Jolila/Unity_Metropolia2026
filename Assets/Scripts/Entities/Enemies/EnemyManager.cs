using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyPoolManager;

public class EnemyManager : MonoBehaviour
{



    Vector3 cachedPlayerPosition;
    [SerializeField] float retargetDistance = 1.5f;
    [SerializeField] EnemyPoolManager _poolManager;
    [SerializeField] EnemySpawner _spawner;
    float spawnInterval = 0.05f;
    float spawnTimer = 0.0f;
    Transform player;

    [System.Serializable]
    public struct UpdatePass
    {
        public PoolID pool;
        public int startIndex;
        public int count;
    }

    [SerializeField]
    private List<UpdatePass> updateSchedule = new();
    private int currentPass;
    bool gameStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        gameStarted = false;
    }

    public void OnStartGame()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cachedPlayerPosition = player.position;
        gameStarted = true;
    }

    public void spawnInitialEnemy()
    {
        GameObject dummy = _spawner.SpawnNewEnemy(null);
        
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameStarted) return;


        if (GameManager.Instance.GetIsCountDown()) return;

        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnInterval)
        {
            GameObject spawned = _spawner.SpawnNewEnemy(cachedPlayerPosition);

            if (spawned != null &&
                spawned.TryGetComponent<IEnemyAI>(out var enemy))
            {
               
                enemy.Tick(cachedPlayerPosition);
            }
            spawnTimer = 0.0f;
        }

        if (
        (player.position - cachedPlayerPosition).sqrMagnitude >
        retargetDistance * retargetDistance)
        {
            cachedPlayerPosition = player.position;
           
        }

        UpdatePass pass = updateSchedule[currentPass];
        UpdateSingularPass(pass);

        currentPass++;

        if (currentPass >= updateSchedule.Count)
            currentPass = 0;

    }




    void UpdateSingularPass(UpdatePass pass)
    {


        Pool pool = EnemyPoolManager.Instance.GetPool(pass.pool);

        int end = Mathf.Min(pass.startIndex + pass.count,
                          pool.objects.Count);


        for (int i = pass.startIndex; i < end; i++)
        {
            GameObject obj = pool.objects[i];

            if (!obj.activeInHierarchy)
                continue;

            IEnemyAI enemy = obj.GetComponent<IEnemyAI>();

            if (enemy == null)
                continue;

            enemy.Tick(cachedPlayerPosition);

           
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
