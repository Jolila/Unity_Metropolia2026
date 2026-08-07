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
    [SerializeField] EnemySpawnLocationsManager spawnLocationsManager;

    public IEnumerator InitializeGamePlayLayer()
    {
        RebuildColliders();


        var sources = new List<NavMeshBuildSource>();

        NavMeshBuilder.CollectSources(
            new Bounds(Vector3.zero, Vector3.one * 1000),
            LayerMask.GetMask("Default"),
            NavMeshCollectGeometry.RenderMeshes,
            0,
            new List<NavMeshBuildMarkup>(),
            sources);

        Debug.Log($"Collected Sources = {sources.Count}");

        // Let Unity rebuild the TilemapCollider
        yield return null;
        Debug.Log(WallsTilemapCollider.shapeCount);
        surface.BuildNavMesh();
        Debug.Log(surface.transform.root.name);

    }

    void RebuildColliders()
    {
        // this should force cache refresh
        WallsTilemapCollider.enabled = false;
        WallsTilemapCollider.enabled = true;
    }

}
