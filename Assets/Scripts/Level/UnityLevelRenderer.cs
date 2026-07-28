using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnityLevelRenderer : MonoBehaviour
{

    [SerializeField] Tilemap LevelOutlineTilemap;
    [SerializeField] Tilemap GroundsTilemap;
    [SerializeField] Tilemap WallsTilemap;
    [SerializeField] Tilemap DecorationsTilemap;
    [SerializeField] TileBase dinoSingularTile;
    [SerializeField] TileBase SkullTile;
    [SerializeField] TileBase outlineTile;

    [SerializeField] RuleTile RandomDinoBoneRuleTile;
    [SerializeField] RuleTile RandomShroomClusterRuleTile;
    [SerializeField] RuleTile RandomSingleShroomRuleTile;

    [SerializeField] RuleTile GroundTile;
    [SerializeField] RuleTile WallTile;




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

    // This is kinda silly and kinda hardcoded to work on the "working" level in that if there is none it will not render anything

    private string GeometryFilePath, DecorationsFilePath, LevelInfoFilePath, OutlineFilePath;
    int levelWidth, levelHeight, outlineWidth, outlineHeight, OutlinePadding;
    char[,] GeometryGrid;
    char[,] DecorationGrid;
    char[,] OutlineGrid;

    void GetAttributes()
    {
        foreach (string line in File.ReadLines(LevelInfoFilePath))
        {
            if (line.StartsWith("width"))
            {
                levelWidth = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("height"))
            {
                levelHeight = int.Parse(line.Split(':')[1].Trim());
            }
        }
    }

        void LoadGeometryGrid()
        {
            string[] lines = File.ReadAllLines(GeometryFilePath);
            for (int y = 0; y < levelHeight; ++y)
            {
                for (int x = 0; x < levelWidth; ++x)
                {
                    GeometryGrid[x, y] = lines[y][x];
                }
            }
        }



    void LoadDecorationGrid()
    {
        string[] lines = File.ReadAllLines(DecorationsFilePath);
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                DecorationGrid[x, y] = lines[y][x];
            }
        }
    }

    void LoadOutlineGrid()
    {
        string[] lines = File.ReadAllLines(OutlineFilePath);

        outlineHeight = lines.Length;
        outlineWidth = lines[0].Length;

        OutlineGrid = new char[outlineWidth, outlineHeight];

        for (int y = 0; y < outlineHeight; y++)
        {
            for (int x = 0; x < outlineWidth; x++)
            {
                OutlineGrid[x, y] = lines[y][x];
            }
        }
    }

    void PreRenderTasks()
    {
        
        // get the level number I am working on now. Suppose only one level only exists in working folder.
        string[] geo = Directory.GetFiles(
        "Assets/Generated/Working",
        "*_geometry.txt");
        GeometryFilePath = geo[0];

        string[] info = Directory.GetFiles(
        "Assets/Generated/Working",
        "*_info.txt");
        LevelInfoFilePath = info[0];

        string[] decor = Directory.GetFiles("Assets/Generated/Working", "*_decorations.txt");
        DecorationsFilePath = decor[0];

        string[] outline = Directory.GetFiles("Assets/Generated/Working", "*_outline.txt");
        OutlineFilePath = outline[0];

        GetAttributes();
        GeometryGrid = new char[levelWidth, levelHeight];
        DecorationGrid = new char[levelWidth, levelHeight];
        LoadGeometryGrid();
        LoadDecorationGrid();
        OutlinePadding = 10;
        LoadOutlineGrid();
    }




    [ContextMenu("Render")]
    public void RenderTilemaps()
    {

        PreRenderTasks();
        RenderGeometry();
        RenderDecorations();
        RenderOutline();
    }

    void RenderOutline()
    {
        for (int y = 0; y < outlineHeight; y++)
        {
            for (int x = 0; x < outlineWidth; x++)
            {
                if (OutlineGrid[x, y] != 'O')
                    continue;

                LevelOutlineTilemap.SetTile(
                    new Vector3Int(
                    x - OutlinePadding,
                    y - OutlinePadding,
                    0),
                    outlineTile);
            }
        }
    }

    void RenderGeometry()
    {
       
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (GeometryGrid[x, y] == '*')
                {
                    GroundsTilemap.SetTile(cell, GroundTile);
                }
                else if (GeometryGrid[x, y] == '#')
                {
                    WallsTilemap.SetTile(cell, WallTile);
                }
   
            }
        }
    }

    void RenderDecorations()
    {

        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (DecorationGrid[x, y] == 'd')
                {
                    DecorationsTilemap.SetTile(cell, RandomDinoBoneRuleTile);
                }
                else if (DecorationGrid[x, y] == 'h')
                {
                    DecorationsTilemap.SetTile(cell, dinoSingularTile);
                }
                else if (DecorationGrid[x,y] == 'C')
                {
                    DecorationsTilemap.SetTile(cell, RandomShroomClusterRuleTile);
                }
                else if (DecorationGrid[x,y] == 'S')
                {
                    DecorationsTilemap.SetTile(cell, RandomSingleShroomRuleTile);
                }
                else if (DecorationGrid[x,y] == 'X')
                {
                    DecorationsTilemap.SetTile(cell, SkullTile);
                }
               
            }
        }
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
