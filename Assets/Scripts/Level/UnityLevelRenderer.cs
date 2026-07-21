using System.IO;
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

    // This is kinda silly and kinda hardcoded to work on the "working" level in that if there is none it will not render anything

    public string GeometryFilePath, DecorationsFilePath, LevelInfoFilePath;
    int levelWidth, levelHeight;
    char[,] levelGeometryGrid;

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

        GetAttributes();

        
    }


    [ContextMenu("Render")]
    public void RenderTilemaps()
    {

        PreRenderTasks(); // Fix infinite recursion by removing from prerendertasks...




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
