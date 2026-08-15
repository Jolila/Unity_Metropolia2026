using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;



public class EnemySpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable()
    {
        GameProgressionManager.Instance.OnCurrentRoundChanged += HandleRoundChanged;
    }


    public void OnStartGame()
    {
        BuildSpawnTargets(GameRound.Round0);
    }


    void Start() {}


    public void SpawnCountDownEnemies() { }


    private float PopulationCheckInterval = 0.25f;
    private float populationCheckTimer;
    public float SpawnQuotaTolerance = 0.1f;
    private SpawnTargets maximumQuotas;
    private SpawnTargets spawnDeficits;

    private struct SpawnTargets
    {
        public int ratSquads;
        public int slimeSquads;
        public int zombieSquads;
        public int bats;
        public int ghosts;
    }

    private struct SpawnDeficits
    {
        public int ratSquads;
        public int slimeSquads;
        public int zombieSquads;
        public int bats;
        public int ghosts;
    }

   

    void Update()
    {
        if (GameManager.Instance.GetState() != GameState.Playing) return;

        if (RewardSystem.Instance.IsActive) return;

        populationCheckTimer -= Time.deltaTime;
        if (populationCheckTimer <= 0f)
        {
            populationCheckTimer = PopulationCheckInterval;
            RefreshSpawnTargets();
        }
        else
        {
            SpawnEnemies();
        }
      

    }

    private void SpawnEnemies()
    {
        if (spawnDeficits.ratSquads > 0)
        {
            SpawnRatSquad();
            spawnDeficits.ratSquads--;
        }

        if (spawnDeficits.slimeSquads > 0)
        {
            SpawnSlimeSquad();
            spawnDeficits.slimeSquads--;
        }

        if (spawnDeficits.zombieSquads > 0)
        {
            SpawnZombieSquad();
            spawnDeficits.zombieSquads--;
        }

        if (spawnDeficits.bats > 0)
        {
            SpawnBats();
            spawnDeficits.bats--;
        }

        if (spawnDeficits.ghosts > 0)
        {
            SpawnGhosts();
            spawnDeficits.ghosts--;
        }
    }




    void SpawnRatSquad()
    {

        Vector3 leaderPos = EnemyManager.Instance.GetEnemySpawnLocationsManager()
            .GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                EnemyType.RatLeader,
                leaderPos,
                Quaternion.identity);

        if (leader == null)
            return;
        EnemyManager.Instance.RegisterEnemy(leader);
        LeaderEnemy leaderEnemy = leader.GetComponent<LeaderEnemy>();

        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2.5f;

            GameObject follower =
                EnemyPoolManager.Instance.Get(
                    EnemyType.RatFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);

            if (follower == null)
                continue;
            EnemyManager.Instance.RegisterEnemy(follower);
            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;
            
        }

    }

    void SpawnGhosts()
    {
        EnemyPoolManager.Instance.Get(EnemyType.Ghost,
            EnemyManager.Instance.GetEnemySpawnLocationsManager()
            .GetRandomInnerWallSpawn(),
            Quaternion.identity,
            GameManager.Instance.GetPlayerReference().transform.position);

    }

    void SpawnRenegadeRats()
    {

    }

    void SpawnRenegadeSlimes()
    {

    }

    void SpawnRenegadeZombies()
    {

    }

    void SpawnBats()
    {

        int amount = Random.Range(0, 7);
        for(int i = 0; i < amount; ++i)
        {
            GameObject bat  = EnemyPoolManager.Instance.Get(EnemyType.Bat,
                EnemyManager.Instance.GetEnemySpawnLocationsManager()
                .GetRandomInnerWallSpawn(),
                Quaternion.identity,
                GameManager.Instance.GetPlayerReference().transform.position);
            if(bat != null) EnemyManager.Instance.RegisterEnemy(bat);
        }
        
    }


    void SpawnSlimeSquad()
    {
        Vector3 leaderPos = EnemyManager.Instance.GetEnemySpawnLocationsManager()
            .GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                EnemyType.SlimeLeader,
                leaderPos,
                Quaternion.identity);

        if (leader == null)
            return;

        LeaderEnemy leaderEnemy = leader.GetComponent<LeaderEnemy>();

        for (int i = 0; i < 10; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2.5f;

            GameObject follower =
                EnemyPoolManager.Instance.Get(
                    EnemyType.SlimeFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);

            if (follower == null)
                continue;

            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;

        }
    }

    void SpawnZombieSquad()
    {
        Vector3 leaderPos = 
            EnemyManager.Instance.GetEnemySpawnLocationsManager().
            GetRandomOuterGroundSpawn();

        GameObject leader =
            EnemyPoolManager.Instance.Get(
                EnemyType.ZombieLeader,
                leaderPos,
                Quaternion.identity);

       

        if (leader == null)
            return;
        EnemyManager.Instance.RegisterEnemy(leader);

        LeaderEnemy leaderEnemy = leader.GetComponent<LeaderEnemy>();

        for (int i = 0; i < 3; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2.5f;

            GameObject follower =
                EnemyPoolManager.Instance.Get(
                    EnemyType.ZombieFollower,
                    leaderPos + (Vector3)offset,
                    Quaternion.identity);
            

            if (follower == null)
                continue;
            EnemyManager.Instance.RegisterEnemy(follower);

            follower.GetComponent<FollowerEnemy>().leader =
                leaderEnemy.transform;

        }
    }




    public void HandleRoundChanged(GameRound newRound)
    {
        BuildSpawnTargets(newRound);
    }



    private void BuildSpawnTargets(GameRound newRound)
    {
        Debug.Log("Spawn targets were built!");
        maximumQuotas = new SpawnTargets();

        RoundDefinition definition = Rounds.Definitions[newRound];

        foreach (var entry in definition.PoolFillPercent)
        {
            EnemyPoolManager.Pool pool =
                EnemyPoolManager.Instance.GetPool(entry.Key);

            if (pool == null)
                continue;

            int maximum = Mathf.FloorToInt(
                pool.objects.Count * entry.Value
            );

            switch (entry.Key)
            {
                case EnemyType.RatLeader:
                    maximumQuotas.ratSquads = maximum;
                    break;

                case EnemyType.SlimeLeader:
                    maximumQuotas.slimeSquads = maximum;
                    break;

                case EnemyType.ZombieLeader:
                    maximumQuotas.zombieSquads = maximum;
                    break;

                case EnemyType.Bat:
                    maximumQuotas.bats = maximum;
                    break;

                case EnemyType.Ghost:
                    maximumQuotas.ghosts = maximum;
                    break;
            }
        }
    }

    private void RefreshSpawnTargets()
    {
        spawnDeficits = new SpawnTargets
        {
            ratSquads = Mathf.Max(
                0,
                maximumQuotas.ratSquads -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.RatLeader)
            ),

            slimeSquads = Mathf.Max(
                0,
                maximumQuotas.slimeSquads -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.SlimeLeader)
            ),

            zombieSquads = Mathf.Max(
                0,
                maximumQuotas.zombieSquads -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.ZombieLeader)
            ),

            bats = Mathf.Max(
                0,
                maximumQuotas.bats -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.Bat)
            ),

            ghosts = Mathf.Max(
                0,
                maximumQuotas.ghosts -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.Ghost)
            )
        };
    }


}
