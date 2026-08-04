using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


public class EnemySpawner : MonoBehaviour
{

    [SerializeField] Tilemap _groundTiles;
    [SerializeField] Tilemap _wallTiles;
    List<Vector3> groundSpawnPositions = new();
    List<Vector3> wallSpawnPositions = new();
    [SerializeField] float cellSize = 12.5f;
    Dictionary<Vector2Int, List<Vector3>> groundsGrid = new();
    Dictionary<Vector2Int, List<Vector3>> wallsGrid = new();
    Vector3 playerPosition;
    Vector2Int currentPlayerCell;
    List<Vector3> groundCandidates = new();
    List<Vector3> groundCandidateBuffer = new();
    List<Vector3> wallCandidates = new();
    List<Vector3> wallCandidateBuffer = new();


    float _currentCooldown;
    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void Initialize()
    {

        SetEnemySpawnPositions();
        BuildSpatialGrid();

    }

    Vector2Int GetCell(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / cellSize),
            Mathf.FloorToInt(worldPos.y / cellSize));
   
    }

    public void BuildSpatialGrid()
    {
        foreach (var point in groundSpawnPositions)
        {
            Vector2Int cell = GetCell(point);

            if(!groundsGrid.TryGetValue(cell, out var list))
            {
                list = new List<Vector3>();
                groundsGrid[cell] = list;
            }
            list.Add(point);
        }

        foreach(var point in wallSpawnPositions)
        {
            Vector2Int cell = GetCell(point);

            if(!wallsGrid.TryGetValue(cell, out var list))
            {
                list = new List<Vector3>();
                wallsGrid[cell] = list;
            }
            list.Add(point);
        }

        RefreshCandidates();
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


    public void SpawnNewEnemy(Vector3? initialTarget)
    {
        HandleEnemySpawning(initialTarget);
    }


    // Update is called once per frame
    void Update()
    {
       
        if (player == null) return;
        playerPosition = player.position;
        Vector2Int playerCell = GetCell(playerPosition);
        if(playerCell != currentPlayerCell)
        {
            currentPlayerCell = playerCell;
            RefreshCandidates();
        }
  
    }

    void RefreshCandidates()
    {
        Debug.Log("Refreshing candidates");
        groundCandidateBuffer.Clear();
        wallCandidateBuffer.Clear();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int cell =
                    currentPlayerCell + new Vector2Int(x, y);

                if (groundsGrid.TryGetValue(cell, out var ground))
                    groundCandidateBuffer.AddRange(ground);

                if (wallsGrid.TryGetValue(cell, out var wall))
                    wallCandidateBuffer.AddRange(wall);
            }
        }
        SwapBuffers();
    }

    void SwapBuffers()
    {
        var tempGround = groundCandidates;
        groundCandidates = groundCandidateBuffer;
        groundCandidateBuffer = tempGround;

        var tempWalls = wallCandidates;
        wallCandidates = wallCandidateBuffer;
        wallCandidateBuffer = tempWalls;
    }

    void HandleEnemySpawning(Vector3? initialTarget)
    {
       
        SpawnEnemyToRandomLocation(initialTarget);

    }

    Vector3 GetRandomPosition(PoolID id)
    {
        bool useWall = id == PoolID.Ghost || id == PoolID.Bat;
        if (useWall) return wallCandidates[Random.Range(0, wallCandidates.Count)];
        return groundCandidates[Random.Range(0, groundCandidates.Count)];
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

    void SpawnEnemyToRandomLocation(Vector3? initialTarget)
    {

        PoolID id = GetRandomEnemyType();
        Vector3 pos = GetRandomPosition(id);
        if(GameManager.Instance.GetIsCountDown())
        {
            PoolManager.Instance.Get(id, pos, Quaternion.identity, null);
            return;
        }
        PoolManager.Instance.Get(id, pos, Quaternion.identity, initialTarget);
       


         
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(playerPosition, new Vector3(cellSize, cellSize, 0));


    }
}
