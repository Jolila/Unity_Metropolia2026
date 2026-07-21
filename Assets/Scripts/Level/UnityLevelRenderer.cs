using UnityEngine;
using UnityEngine.Tilemaps;

public class UnityLevelRenderer : MonoBehaviour
{

    [SerializeField] Tilemap LevelOutlineTilemap;
    [SerializeField] Tilemap GroundsTilemap;
    [SerializeField] Tilemap WallsTilemap;
    [SerializeField] Tilemap DecorationsTilemap;

    // at this point, the textual data exists

    [ContextMenu("Render")]
    public void RenderTilemaps()
    {

    }
}
