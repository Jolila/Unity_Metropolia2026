using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCleaner : MonoBehaviour
{

    [ContextMenu("Clear tilemaps")]
    void clear()
    {
        foreach (var tilemap in GetComponentsInChildren<Tilemap>()) tilemap.ClearAllTiles();

    }
}
