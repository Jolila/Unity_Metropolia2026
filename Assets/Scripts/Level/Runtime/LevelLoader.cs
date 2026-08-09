using System.Collections;
using System.IO;
using UnityEngine;


public class LevelLoader : MonoBehaviour
{

    public static LevelLoader Instance { get; private set; }

    public LevelData CurrentLevel { get; private set; }

    [SerializeField] LevelRenderer _renderer;
    [SerializeField] LevelGamePlayLoader _gameplayloader;

    void Awake()
    {
        Instance = this;
    }


    private string GeometryFilePath, DecorationsFilePath, LevelInfoFilePath, OutlineFilePath;
    int levelWidth, levelHeight, playerX, playerY;


    public IEnumerator LoadLevel()
    {
        CurrentLevel = new LevelData();
        _renderer.ClearTilemaps();
        LoadNewLevel();
        yield return null;
        _renderer.RenderLevel(CurrentLevel);
        yield return null;

        yield return StartCoroutine(
       _gameplayloader.InitializeGamePlayLayer());

    }

    private void LoadNewLevel()
    {


        // runtime asset management
        string readyFolder = Path.Combine(
        Application.streamingAssetsPath,
        "Levels");

        string[] levelFolders = Directory.GetDirectories(readyFolder);
        string levelFolder = levelFolders[Random.Range(0, levelFolders.Length)];

        GeometryFilePath = Path.Combine(levelFolder, "geometry.txt");
        DecorationsFilePath = Path.Combine(levelFolder, "decorations.txt");
        LevelInfoFilePath = Path.Combine(levelFolder, "info.txt");
        OutlineFilePath = Path.Combine(levelFolder, "outline.txt");



        // Loading -> load to struct

        ReadInfo();
        CurrentLevel.Width = levelWidth;
        CurrentLevel.Height = levelHeight;
        CurrentLevel.PlayerX = playerX;
        CurrentLevel.PlayerY = playerY;

        LoadGeometryGrid();
        LoadDecorationGrid();
        LoadOutlineGrid();
        GameManager.Instance.GetPlayerReference().transform.position = new Vector3(playerX, playerY, 0);
        CurrentLevel.PlayerX = playerX;
        CurrentLevel.PlayerY = playerY;

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



    void LoadOutlineGrid()
    {
        string[] lines = File.ReadAllLines(OutlineFilePath);

        CurrentLevel.OutlineHeight = lines.Length;
        CurrentLevel.OutlineWidth = lines[0].Length;

        CurrentLevel.Outline = new char[CurrentLevel.OutlineWidth, CurrentLevel.OutlineHeight];

        for (int y = 0; y < CurrentLevel.OutlineHeight; y++)
        {
            for (int x = 0; x < CurrentLevel.OutlineWidth; x++)
            {
                CurrentLevel.Outline[x, y] = lines[y][x];
            }
        }
    }



    void LoadGeometryGrid()
    {
        CurrentLevel.Geometry = new char[levelWidth, levelHeight];

        string[] lines = File.ReadAllLines(GeometryFilePath);
        for (int y = 0; y < levelHeight; ++y)
        {
           for (int x = 0; x < levelWidth; ++x)
           {
              CurrentLevel.Geometry[x, y] = lines[y][x];
           }
        }
    }

    void LoadDecorationGrid()
    {


        CurrentLevel.Decorations = new char[levelWidth, levelHeight];
        string[] lines = File.ReadAllLines(DecorationsFilePath);
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                CurrentLevel.Decorations[x, y] = lines[y][x];
            }
        }
    }




}
