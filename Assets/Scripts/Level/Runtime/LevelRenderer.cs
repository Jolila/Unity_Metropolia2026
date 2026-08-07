using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelRenderer : MonoBehaviour
{

    [SerializeField] Tilemap DecorationsTilemap;
    [SerializeField] TileBase dinoSingularTile;
    [SerializeField] TileBase SkullTile;
    [SerializeField] Tilemap OutlineDecorationsTilemap;
    [SerializeField] TileBase outlineTile;

    [SerializeField] Tilemap LevelOutlineTilemap;
    [SerializeField] Tilemap GroundsTilemap;
    [SerializeField] Tilemap WallsTilemap;

    [SerializeField] RuleTile RandomDinoBoneRuleTile;
    [SerializeField] RuleTile RandomShroomClusterRuleTile;
    [SerializeField] RuleTile RandomSingleShroomRuleTile;

    [SerializeField] RuleTile GroundTile;
    [SerializeField] RuleTile WallTile;

    public void ClearTilemaps()
    {
        //Rendering pretask
        GroundsTilemap.ClearAllTiles();
        WallsTilemap.ClearAllTiles();
        DecorationsTilemap.ClearAllTiles();
        LevelOutlineTilemap.ClearAllTiles();
    }

    public void RenderLevel(LevelData level)
    {
        //Rendering

  

        RenderGeometry(level);
        RenderDecorations(level);
        RenderOutline(level);


    }

    void RenderGeometry(LevelData level)
    {
        int grounds = 0;
        int walls = 0;


        for (int y = 0; y < level.Height; ++y)
        {
            for (int x = 0; x < level.Width; ++x)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (level.Geometry[x, y] == '*')
                {
                    GroundsTilemap.SetTile(cell, GroundTile);
                    ++grounds;
                }
                else if (level.Geometry[x, y] == '#')
                {
                    WallsTilemap.SetTile(cell, WallTile);
                    ++walls;
                }

            }
        }


    }

    void RenderDecorations(LevelData level)
    {

        for (int y = 0; y < level.Height; ++y)
        {
            for (int x = 0; x < level.Width; ++x)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (level.Decorations[x, y] == 'd')
                {
                    DecorationsTilemap.SetTile(cell, RandomDinoBoneRuleTile);
                }
                else if (level.Decorations[x, y] == 'h')
                {
                    DecorationsTilemap.SetTile(cell, dinoSingularTile);
                }
                else if (level.Decorations[x, y] == 'C')
                {
                    DecorationsTilemap.SetTile(cell, RandomShroomClusterRuleTile);
                }
                else if (level.Decorations[x, y] == 'S')
                {
                    DecorationsTilemap.SetTile(cell, RandomSingleShroomRuleTile);
                }
                else if (level.Decorations[x, y] == 'X')
                {
                    DecorationsTilemap.SetTile(cell, SkullTile);
                }

            }
        }
    }



    void RenderOutline(LevelData level)
    {



        for (int y = 0; y < level.OutlineHeight; y++)
        {
            for (int x = 0; x < level.OutlineWidth; x++)
            {
                Vector3Int cell = new Vector3Int(x - level.OutlinePadding, y - level.OutlinePadding, 0);
                LevelOutlineTilemap.SetTile(cell, outlineTile);
            }
        }

        for (int y = 0; y < level.OutlineHeight; y++)
        {
            for (int x = 0; x < level.OutlineWidth; x++)
            {
                Vector3Int cell = new Vector3Int(x - level.OutlinePadding, y - level.OutlinePadding, 0);
                if (level.Outline[x, y] == 'C')
                {
                    OutlineDecorationsTilemap.SetTile(cell, RandomShroomClusterRuleTile);
                }
                else if (level.Outline[x, y] == 'S')
                {
                    OutlineDecorationsTilemap.SetTile(cell, RandomSingleShroomRuleTile);
                }
            }
        }





    }



}
