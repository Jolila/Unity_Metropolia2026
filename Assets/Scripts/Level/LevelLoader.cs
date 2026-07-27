using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LevelRenderer : MonoBehaviour
{

    private string GeometryFilePath, DecorationsFilePath, LevelInfoFilePath;
    int levelWidth, levelHeight, playerX, playerY;
    char[,] GeometryGrid;
    char[,] DecorationGrid;

    [SerializeField] Tilemap LevelOutlineTilemap;
    [SerializeField] Tilemap GroundsTilemap;
    [SerializeField] Tilemap WallsTilemap;
    [SerializeField] Tilemap DecorationsTilemap;
    [SerializeField] TileBase dinoSingularTile;
    [SerializeField] TileBase SkullTile;

    [SerializeField] RuleTile RandomDinoBoneRuleTile;
    [SerializeField] RuleTile RandomShroomClusterRuleTile;
    [SerializeField] RuleTile RandomSingleShroomRuleTile;

    [SerializeField] RuleTile GroundTile;
    [SerializeField] RuleTile WallTile;
    [SerializeField] GameObject player;


    private int width;
    private int height;

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {

        string readyFolder = "Assets/Generated/Ready";

        string[] levelFolders = Directory.GetDirectories(readyFolder);

        string levelFolder = levelFolders[0];

        GeometryFilePath = Path.Combine(levelFolder, "geometry.txt");
        DecorationsFilePath = Path.Combine(levelFolder, "decorations.txt");
        LevelInfoFilePath = Path.Combine(levelFolder, "info.txt");

        GroundsTilemap.ClearAllTiles();
        WallsTilemap.ClearAllTiles();
        DecorationsTilemap.ClearAllTiles();
        ReadInfo();
        LoadGeometryGrid();
        LoadDecorationGrid();
        RenderGeometry();
        RenderDecorations();
        player.transform.position = new Vector3(playerX, playerY, 0);


    }

    void ReadInfo()
    {

        foreach(string line in File.ReadLines(LevelInfoFilePath))
        {
            string[] parts = line.Split(':');

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case "width":
                    levelWidth = int.Parse(value);
                    break;

                case "height":
                    levelHeight = int.Parse(value);
                    break;

                case "playerX":
                    playerX = int.Parse(value);
                    break;

                case "playerY":
                    playerY = int.Parse(value);
                    break;
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
                else if (DecorationGrid[x, y] == 'C')
                {
                    DecorationsTilemap.SetTile(cell, RandomShroomClusterRuleTile);
                }
                else if (DecorationGrid[x, y] == 'S')
                {
                    DecorationsTilemap.SetTile(cell, RandomSingleShroomRuleTile);
                }
                else if (DecorationGrid[x, y] == 'X')
                {
                    DecorationsTilemap.SetTile(cell, SkullTile);
                }

            }
        }
    }


}
