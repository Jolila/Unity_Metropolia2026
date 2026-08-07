using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyPoolManager;

public class EnemyManager : MonoBehaviour
{

    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyManager>();
                if (_instance == null)
                {
                    Debug.Log(" Error : no enemy manager instance");
                }
            }
            return _instance;
        }
    }


    Vector3 cachedPlayerPosition;
    [SerializeField] float retargetDistance = 1.5f;
    [SerializeField] EnemyPoolManager _poolManager;
    [SerializeField] EnemySpawner _spawner;

    [SerializeField] float cellSize = 12.5f;
    private readonly Dictionary<Vector2Int, List<GameObject>> enemyGrid = new();
    private readonly Dictionary<GameObject, Vector2Int> enemyCells = new();

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
                UpdateEnemyCell(obj);
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

    Vector2Int GetCell(Vector3 position)
    {
        return new Vector2Int(
     Mathf.FloorToInt(position.x / cellSize),
     Mathf.FloorToInt(position.y / cellSize));
    }

    public void RegisterEnemy(GameObject enemy)
    {
        Vector2Int cell = GetCell(enemy.transform.position);

        if (!enemyGrid.TryGetValue(cell, out var list))
        {
            list = new List<GameObject>();
            enemyGrid[cell] = list;
        }

        list.Add(enemy);
        enemyCells[enemy] = cell;
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (!enemyCells.TryGetValue(enemy, out var cell))
            return;

        if (enemyGrid.TryGetValue(cell, out var list))
        {
            list.Remove(enemy);

            if (list.Count == 0)
                enemyGrid.Remove(cell);
        }

        enemyCells.Remove(enemy);
    }

    public void UpdateEnemyCell(GameObject enemy)
    {
        Vector2Int newCell = GetCell(enemy.transform.position);

        if (!enemyCells.TryGetValue(enemy, out var oldCell))
        {
            RegisterEnemy(enemy);
            return;
        }

        if (oldCell == newCell)
            return;

        enemyGrid[oldCell].Remove(enemy);

        if (enemyGrid[oldCell].Count == 0)
            enemyGrid.Remove(oldCell);

        if (!enemyGrid.TryGetValue(newCell, out var list))
        {
            list = new List<GameObject>();
            enemyGrid[newCell] = list;
        }

        list.Add(enemy);

        enemyCells[enemy] = newCell;
    }

    public void GetEnemiesInCell(Vector3 position, List<GameObject> results)
    {

        results.Clear();
        if (enemyGrid.TryGetValue(GetCell(position), out var list)) results.AddRange(list);
   
    }

    public void GetNeighboringEnemies(Vector3 position, List<GameObject> results)
    {
        results.Clear();


        Vector2Int center = GetCell(position);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int cell = center + new Vector2Int(x, y);

                if (enemyGrid.TryGetValue(cell, out var list))
                    results.AddRange(list);
            }
        }
    }

    //void OnDrawGizmosSelected()
    //{
    //    if (!Application.isPlaying)
    //        return;

    //    Gizmos.color = Color.purple;
    //    Gizmos.DrawSphere(cachedPlayerPosition, retargetDistance);



    //}


}
