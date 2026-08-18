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
        public int RatSquads;
        public int SlimeSquads;
        public int Zombies;
        public int Bats;
        public int Ghosts;
    }

    private struct SpawnDeficits
    {
        public int RatSquads;
        public int SlimeSquads;
        public int Zombies;
        public int Bats;
        public int Ghosts;
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
        if (spawnDeficits.RatSquads > 0)
        {
            SpawnRatSquad();
            spawnDeficits.RatSquads--;
        }

        if (spawnDeficits.SlimeSquads > 0)
        {
            SpawnSlimeSquad();
            spawnDeficits.SlimeSquads--;
        }

        if (spawnDeficits.Zombies > 0)
        {
            SpawnZombie();
            spawnDeficits.Zombies--;
        }

        if (spawnDeficits.Bats > 0)
        {
            SpawnBats();
            spawnDeficits.Bats--;
        }

        if (spawnDeficits.Ghosts > 0)
        {
            SpawnGhosts();
            spawnDeficits.Ghosts--;
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

       Vector3 spawnPos =
       EnemyManager.Instance.GetEnemySpawnLocationsManager().
       GetRandomInnerWallSpawn();

        GameObject ghost =
            EnemyPoolManager.Instance.Get(
                EnemyType.Ghost,
                spawnPos,
                Quaternion.identity,
                GameManager.Instance.GetPlayerReference().transform.position);

        if (ghost == null) return;
        EnemyManager.Instance.RegisterEnemy(ghost);

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

        int amount = Random.Range(0, 3);
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

        for (int i = 0; i < 4; i++)
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

    void SpawnZombie()
    {
        Vector3 spawnPos = 
            EnemyManager.Instance.GetEnemySpawnLocationsManager().
            GetRandomInnerGroundSpawn();

        GameObject zom =
            EnemyPoolManager.Instance.Get(
                EnemyType.Zombie,
                spawnPos,
                Quaternion.identity);


        if (zom == null) return;
        EnemyManager.Instance.RegisterEnemy(zom);
      
    }




    public void HandleRoundChanged(GameRound newRound)
    {
        BuildSpawnTargets(newRound);
    }



    private void BuildSpawnTargets(GameRound newRound)
    {
        
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
                    maximumQuotas.RatSquads = maximum;
                    break;

                case EnemyType.SlimeLeader:
                    maximumQuotas.SlimeSquads = maximum;
                    break;

                case EnemyType.Zombie:
                    maximumQuotas.Zombies = maximum;
                    break;

                case EnemyType.Bat:
                    maximumQuotas.Bats = maximum;
                    break;

                case EnemyType.Ghost:
                    maximumQuotas.Ghosts = maximum;
                    break;
            }
        }
    }

    private void RefreshSpawnTargets()
    {
        spawnDeficits = new SpawnTargets
        {
            RatSquads = Mathf.Max(
                0,
                maximumQuotas.RatSquads -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.RatLeader)
            ),

            SlimeSquads = Mathf.Max(
                0,
                maximumQuotas.SlimeSquads -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.SlimeLeader)
            ),

            Zombies = Mathf.Max(
                0,
                maximumQuotas.Zombies -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.Zombie)
            ),

            Bats = Mathf.Max(
                0,
                maximumQuotas.Bats -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.Bat)
            ),

            Ghosts = Mathf.Max(
                0,
                maximumQuotas.Ghosts -
                EnemyPoolManager.Instance.GetActiveCount(EnemyType.Ghost)
            )
        };
    }


}
