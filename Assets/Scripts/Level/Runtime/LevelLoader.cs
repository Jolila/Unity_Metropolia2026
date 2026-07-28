using NavMeshPlus.Components;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelRenderer : MonoBehaviour
{

    private string GeometryFilePath, DecorationsFilePath, LevelInfoFilePath, OutlineFilePath;
    int levelWidth, levelHeight, playerX, playerY, outlineWidth, outlineHeight;
    char[,] GeometryGrid;
    char[,] DecorationGrid;
    char[,] OutlineGrid;
    int OutlinePadding = 10;

    [SerializeField] Tilemap LevelOutlineTilemap;
    [SerializeField] Tilemap GroundsTilemap;
    [SerializeField] Tilemap WallsTilemap;
    [SerializeField] TilemapCollider2D WallsTilemapCollider;
    [SerializeField] NavMeshSurface surface;
    [SerializeField] EnemySpawner spawner;
    [SerializeField] Tilemap DecorationsTilemap;
    [SerializeField] TileBase dinoSingularTile;
    [SerializeField] TileBase SkullTile;
    [SerializeField] Tilemap OutlineDecorationsTilemap;
    [SerializeField] TileBase outlineTile;

    [SerializeField] RuleTile RandomDinoBoneRuleTile;
    [SerializeField] RuleTile RandomShroomClusterRuleTile;
    [SerializeField] RuleTile RandomSingleShroomRuleTile;

    [SerializeField] RuleTile GroundTile;
    [SerializeField] RuleTile WallTile;
    [SerializeField] GameObject player;


    private int width;
    private int height;

    private void Awake()
    {
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        LoadLevel();
        yield return null;

        RebuildColliders();
        surface.BuildNavMesh();
        spawner = FindAnyObjectByType<EnemySpawner>();
        spawner.Initialize();

    }

    private void LoadLevel()
    {

        string readyFolder = Path.Combine(
        Application.streamingAssetsPath,
        "Levels");

        string[] levelFolders = Directory.GetDirectories(readyFolder);

        string levelFolder = levelFolders[0];

        GeometryFilePath = Path.Combine(levelFolder, "geometry.txt");
        DecorationsFilePath = Path.Combine(levelFolder, "decorations.txt");
        LevelInfoFilePath = Path.Combine(levelFolder, "info.txt");
        OutlineFilePath = Path.Combine(levelFolder, "outline.txt");


        GroundsTilemap.ClearAllTiles();
        WallsTilemap.ClearAllTiles();
        DecorationsTilemap.ClearAllTiles();
        LevelOutlineTilemap.ClearAllTiles();
        ReadInfo();
        GeometryGrid = new char[levelWidth, levelHeight];
        DecorationGrid = new char[levelWidth, levelHeight];
        LoadGeometryGrid();
        LoadDecorationGrid();
        LoadOutlineGrid();
        player.transform.position = new Vector3(playerX, playerY, 0);
        RenderOutline();
        RenderGeometry();
        RenderDecorations();


    }

    void RebuildColliders()
    {
        // this should force cache refresh
        WallsTilemapCollider.enabled = false;
        WallsTilemapCollider.enabled = true;
    }



    

    void ReadInfo()
    {

        foreach(string line in File.ReadLines(LevelInfoFilePath))
        {
            string[] parts = line.Split(':');

            if (parts.Length != 2)
                continue; // islet count line, I dunno if its needed but lets keep it in file

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

    void RenderOutline()
    {
        

        for (int y = 0; y < outlineHeight; y++)
        {
            for (int x = 0; x < outlineWidth; x++)
            {
                Vector3Int cell = new Vector3Int(x - OutlinePadding, y - OutlinePadding, 0);
                LevelOutlineTilemap.SetTile(cell, outlineTile);
            }
        }

        for (int y = 0; y < outlineHeight; y++)
        {
            for (int x = 0; x < outlineWidth; x++)
            {
                Vector3Int cell = new Vector3Int(x - OutlinePadding, y - OutlinePadding, 0);
                if (OutlineGrid[x, y] == 'C')
                {
                    OutlineDecorationsTilemap.SetTile(cell, RandomShroomClusterRuleTile);
                }
                else if (OutlineGrid[x, y] == 'S')
                {
                    OutlineDecorationsTilemap.SetTile(cell, RandomSingleShroomRuleTile);
                }
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
