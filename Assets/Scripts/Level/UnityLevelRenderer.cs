using UnityEngine;
using UnityEngine.Tilemaps;

public class UnityLevelRenderer : MonoBehaviour
{

    [SerializeField] Tilemap LevelOutlineTilemap;
    [SerializeField] Tilemap GroundsTilemap;
    [SerializeField] Tilemap WallsTilemap;
    [SerializeField] Tilemap DecorationsTilemap;

    [SerializeField] RuleTile RandomDinoBoneRuleTile;




    /**
   * ENCODINGS:
      C shroom cluster
      S single shroom
      E for embellishment tile
      D other dino bone elements in pallette, lets try this for now
      X for skull in ground (rare)
      Y for full dinosaur skeleton (big)
      and as previously # marks wall tile, * marks ground tile
   */

    // at this point, the textual data exists

    [ContextMenu("Render")]
    public void RenderTilemaps()
    {


    }

    [ContextMenu("Clear tilemaps")]
    public void Clear()
    {
        LevelOutlineTilemap.ClearAllTiles();
        GroundsTilemap.ClearAllTiles();
        WallsTilemap.ClearAllTiles();
        DecorationsTilemap.ClearAllTiles();
    }
}
