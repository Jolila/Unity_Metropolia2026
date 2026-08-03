using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


public class EnemySpawner : MonoBehaviour
{

    [SerializeField] Tilemap _groundTiles;
    [SerializeField] Tilemap _wallTiles;
    List<Vector3> groundSpawnPositions = new();
    List<Vector3> wallSpawnPositions = new();
    [SerializeField] float _spawnCooldown;
    [SerializeField] float _spawnCooldownReductionMultiplier;
    Vector3 playerPosition;
    double minimumDistance;
    float _currentCooldown;
    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        minimumDistance = 1.25f;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void Initialize()
    {
        SetEnemySpawnPositions();
        InvokeRepeating(nameof(HandleGameDifficultyIncrease), 1f, 1f);

        // initial rats or some warm up objects, maybe there should be less and they do not move
        //for(int i = 0; i < 20; ++i)
        //{
        //    Vector3 spawnPos = GetRandomPosition(PoolID.Slime);
        //    PoolManager.Instance.Get(PoolID.Rat, spawnPos, Quaternion.identity);
        //}
    }


    void SetEnemySpawnPositions()
    {
        foreach(Vector3Int position in _groundTiles.cellBounds.allPositionsWithin)
        {
            if(_groundTiles.HasTile(position))
            {
                groundSpawnPositions.Add(_groundTiles.GetCellCenterWorld(position));
            }
        }

        foreach(Vector3Int p in _wallTiles.cellBounds.allPositionsWithin)
        {
            if(_wallTiles.HasTile(p))
            {
                wallSpawnPositions.Add(_wallTiles.GetCellCenterWorld(p));
            }
        }
    }

    void HandleGameDifficultyIncrease()
    {
        _spawnCooldown *= _spawnCooldownReductionMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        playerPosition = player.position;
        HandleEnemySpawning();
    }

    void HandleEnemySpawning()
    {
        _currentCooldown -= Time.deltaTime;
        if (_currentCooldown > Time.time) return;

        _currentCooldown = Time.time + _spawnCooldown;
        SpawnEnemyToRandomLocation();

    }

    Vector3 GetRandomPosition(PoolID id)
    {
        Vector3 spawnPosition = groundSpawnPositions[0];
        bool useWall = id == PoolID.Ghost || id == PoolID.Bat;
        const int maxIters = 3;

        if (!useWall)
        {
            for (int i = 0; i <= maxIters; i++)
            {
                spawnPosition = groundSpawnPositions[Random.Range(0, groundSpawnPositions.Count)];
                Vector3 toPlayer = spawnPosition - playerPosition;
                if (toPlayer.magnitude > minimumDistance)
                {
                    return spawnPosition;
                }
            }
        }

        else
        {
            for (int i = 0; i <= maxIters; i++)
            {


                spawnPosition = wallSpawnPositions[Random.Range(0, wallSpawnPositions.Count)];
                Vector3 toPlayer = spawnPosition - playerPosition;
                if (toPlayer.magnitude > minimumDistance)
                {
                    return spawnPosition;
                }
            }
        }

            return spawnPosition;
    }

    PoolID GetRandomEnemyType()
    {
        int n = Random.Range(0, 5);
        return n switch
        {
            0 => PoolID.Rat,
            1 => PoolID.Bat,
            2 => PoolID.Slime,
            3 => PoolID.Zombie,
            4 => PoolID.Ghost
        };
    }

    void SpawnEnemyToRandomLocation()
    {

        PoolID id = GetRandomEnemyType();
        Vector3 pos = GetRandomPosition(id);
        PoolManager.Instance.Get(id, pos, Quaternion.identity);

        //if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        //{
           
        //    return;
        //}

        //PoolManager.Instance.Get(id, pos, Quaternion.identity);
            
    }
}
