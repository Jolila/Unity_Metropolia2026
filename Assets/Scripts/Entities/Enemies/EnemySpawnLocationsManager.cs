using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


public class EnemySpawnLocationsManager : MonoBehaviour
{

    [SerializeField] Tilemap _groundTiles;
    [SerializeField] Tilemap _wallTiles;
    List<Vector3> groundSpawnPositions = new();
    List<Vector3> wallSpawnPositions = new();
    [SerializeField] float cellSize = 12.5f;
    Dictionary<Vector2Int, List<Vector3>> groundsGrid = new();
    Dictionary<Vector2Int, List<Vector3>> wallsGrid = new();

    Vector2Int currentPlayerCell;
    List<Vector3> innerGroundCandidates = new();
    List<Vector3> outerGroundCandidates = new();

    List<Vector3> innerWallCandidates = new();
    List<Vector3> outerWallCandidates = new();


    float _currentCooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    public void Initialize()
    {

        SetEnemySpawnPositions();
        BuildSpatialGrid();
        Debug.Log("initialize was called before asking for spawn points");
        Debug.Log("initialize was called before asking for spawn points");
        Debug.Log("initialize was called before asking for spawn points");
        

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


    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.GetState() != GameState.Countdown || GameManager.Instance.GetState() != GameState.Playing) return; // OR later
        
        Vector2Int playerCell = GetCell(GameManager.Instance.GetPlayerReference().transform.position);
        if(playerCell != currentPlayerCell)
        {
            currentPlayerCell = playerCell;
            RefreshCandidates();
        }
  
    }

    void RefreshCandidates()
    {
        innerGroundCandidates.Clear();
        outerGroundCandidates.Clear();

        innerWallCandidates.Clear();
        outerWallCandidates.Clear();

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int cell =
                    currentPlayerCell + new Vector2Int(x, y);

                int ring = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                if (groundsGrid.TryGetValue(cell, out var grounds))
                {
                    if (ring == 1)
                        innerGroundCandidates.AddRange(grounds);
                    else
                        outerGroundCandidates.AddRange(grounds);
                }

                if (wallsGrid.TryGetValue(cell, out var walls))
                {
                    if (ring == 1)
                        innerWallCandidates.AddRange(walls);
                    else
                        outerWallCandidates.AddRange(walls);
                }
            }
        }
        
    }


    public Vector3 GetRandomOuterGroundSpawn()
    {
        Debug.Log(outerGroundCandidates.Count + " of outer ground candidates");
        return outerGroundCandidates[
            Random.Range(0, outerGroundCandidates.Count)];   
    }

    public Vector3 GetRandomInnerGroundSpawn()
    {
        return innerGroundCandidates[
            Random.Range(0, innerGroundCandidates.Count)];
    }

    public Vector3 GetRandomOuterWallSpawn()
    {
        return outerWallCandidates[
            Random.Range(0, outerWallCandidates.Count)];
    }

    public Vector3 GetRandomInnerWallSpawn()
    {
        return innerWallCandidates[
            Random.Range(0, innerWallCandidates.Count)];
    }

    //void OnDrawGizmosSelected()
    //{
    //    if (!Application.isPlaying)
    //        return;

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawCube(playerPosition, new Vector3(cellSize, cellSize, 0));


    //}
}
