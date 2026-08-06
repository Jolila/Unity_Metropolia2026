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

    [System.Serializable]
    public struct UpdatePass
    {
        public int ratLeaders;
        public int slimeLeaders;
        public int zombieLeaders;

        public int followers;

        public int bats;
        public int ghosts;
    }

    private readonly Dictionary<EnemyType, int> updateCursors = new();

    [SerializeField]
    private UpdatePass defaultUpdatePass = new()
    {
        ratLeaders = 2,
        slimeLeaders = 2,
        zombieLeaders = 2,

        followers = 30,

        bats = 20,
        ghosts = 20
    };



    [SerializeField]
    private List<UpdatePass> updateSchedule = new();
    private int currentPass;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { }


    public void OnStartGame()
    {
        cachedPlayerPosition = GameManager.Instance.GetPlayerReference().transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Countdown) _spawner.SpawnCountDownEnemies();
        
        if (GameManager.Instance.GetState() != GameState.Playing) return;
        Vector3 currentPlayerPos = GameManager.Instance.GetPlayerReference().transform.position;
        if (
        (currentPlayerPos - cachedPlayerPosition).sqrMagnitude >
        retargetDistance * retargetDistance)
        {
            cachedPlayerPosition = currentPlayerPos;
        }

        //UpdatePass pass = updateSchedule[currentPass];



        UpdateSingularPass(defaultUpdatePass);

        //currentPass++;

        //if (currentPass >= updateSchedule.Count)
        //    currentPass = 0;

    }



    void UpdateSingularPass(UpdatePass pass)
    {

        UpdatePool(EnemyType.RatLeader, pass.ratLeaders);
        UpdatePool(EnemyType.SlimeLeader, pass.slimeLeaders);
        UpdatePool(EnemyType.ZombieLeader, pass.zombieLeaders);

        UpdatePool(EnemyType.RatFollower, pass.followers);
        UpdatePool(EnemyType.SlimeFollower, pass.followers);
        UpdatePool(EnemyType.ZombieFollower, pass.followers);

        UpdatePool(EnemyType.Bat, pass.bats);
        UpdatePool(EnemyType.Ghost, pass.ghosts);

    }


    private void UpdatePool(EnemyType type, int budget)
    {
        if (budget <= 0)
            return;

        Pool pool = _poolManager.GetPool(type);

        if (pool == null || pool.objects.Count == 0)
            return;

        if (!updateCursors.TryGetValue(type, out int cursor))
            cursor = 0;

        int updated = 0;
        int searched = 0;

        while (updated < budget && searched < pool.objects.Count)
        {
            if (cursor >= pool.objects.Count)
                cursor = 0;

            GameObject obj = pool.objects[cursor];

            cursor++;
            searched++;

            if (!obj.activeInHierarchy)
                continue;

            if (obj.TryGetComponent<IEnemyAI>(out var enemy))
            {
                enemy.Tick(cachedPlayerPosition);
                updated++;
            }
        }

        updateCursors[type] = cursor;
    }



    public void ResetScheduler()
    {
        updateCursors.Clear();

        foreach (EnemyType type in System.Enum.GetValues(typeof(EnemyType)))
            updateCursors[type] = 0;
    }

    //void OnDrawGizmosSelected()
    //{
    //    if (!Application.isPlaying)
    //        return;

    //    Gizmos.color = Color.purple;
    //    Gizmos.DrawSphere(cachedPlayerPosition, retargetDistance);



    //}


}
