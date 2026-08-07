using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class LevelGamePlayLoader : MonoBehaviour
{

    [SerializeField] TilemapCollider2D WallsTilemapCollider;
    [SerializeField] NavMeshSurface surface;
    [SerializeField] EnemySpawnLocationsManager _spawnLocationsManager;

    public IEnumerator InitializeGamePlayLayer()
    {
        RebuildColliders();


        // Let Unity rebuild the TilemapCollider
        yield return null;
 
        surface.BuildNavMesh();

        _spawnLocationsManager = FindAnyObjectByType<EnemySpawnLocationsManager>();
        _spawnLocationsManager.Initialize();

    }

    void RebuildColliders()
    {
        // this should force cache refresh
        WallsTilemapCollider.enabled = false;
        WallsTilemapCollider.enabled = true;
    }

}
