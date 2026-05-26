using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] Tilemap _groundTiles;
    List<Vector3> _spawnPositions = new();
    [SerializeField] Enemy _ratPrefab;
    [SerializeField] Enemy _batPrefab;
    [SerializeField] Enemy _slimePrefab;

    List<Enemy> enemies = new List<Enemy>();
    [SerializeField] float _spawnCooldown;
    [SerializeField] float _spawnCooldownReductionMultiplier;
    float _currentCooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies.Add(_ratPrefab);
        enemies.Add(_batPrefab);
        enemies.Add(_slimePrefab);
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
        return _spawnPositions[Random.Range(0, _spawnPositions.Count)];
    }

    void SpawnEnemyToRandomLocation()
    {

        GameObject enemy = ObjectPool.SharedInstance.GetPooledObject();
        if(enemy != null)
        {
            enemy.transform.position = GetRandomPosition();
            enemy.SetActive(true);
        }
        /*
         * 
            Instantiate(enemies[(Random.Range(0, enemies.Count))]
                , GetRandomPosition(), Quaternion.identity);
        */
        
       
        
         
            
    }
}
