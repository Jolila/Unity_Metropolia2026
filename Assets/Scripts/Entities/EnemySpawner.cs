using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


public class EnemySpawner : MonoBehaviour
{

    [SerializeField] Tilemap _groundTiles;
    List<Vector3> _spawnPositions = new();
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
    }


    void SetEnemySpawnPositions()
    {
        foreach(Vector3Int position in _groundTiles.cellBounds.allPositionsWithin)
        {
            if(_groundTiles.HasTile(position))
            {
                _spawnPositions.Add(_groundTiles.GetCellCenterWorld(position));
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

    Vector3 GetRandomPosition()
    {
        Vector3 spawnPosition = _spawnPositions[0];
   

        const int maxIters = 3;

        for(int i = 0; i <= maxIters; i++)
        {
            spawnPosition = _spawnPositions[Random.Range(0, _spawnPositions.Count)];
            Vector3 toPlayer = spawnPosition - playerPosition;
            if(toPlayer.magnitude > minimumDistance)
            {
                return spawnPosition;
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
        Vector3 pos = GetRandomPosition();

        if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            Debug.Log("Rejected spawn - not on NavMesh");
            return;
        }

        PoolManager.Instance.Get(id, pos, Quaternion.identity);
            
    }
}
