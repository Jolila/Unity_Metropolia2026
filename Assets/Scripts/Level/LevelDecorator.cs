using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelDecorator : MonoBehaviour
{


    [SerializeField] Tilemap DecorationTilemap;


    private static readonly Vector2Int[] patternCoordinates =
{
    new(-1,  1),
    new( 0,  1),
    new( 1,  1),

    new(-1,  0),
    new( 0,  0),
    new( 1,  0),

    new(-1, -1),
    new( 0, -1),
    new( 1, -1)
};

    char[,] levelGrid; // Take in only the grid data since the decoration generation is not based on the rules
    // alternatively for placing the dino tiles a sweep for tiles that can house the dino full skeleton needs to happen


    [SerializeField] int level_No = 0;
    int levelWidth, levelHeight, seed;

    void GetAttributes()
    {
        foreach (string line in File.ReadLines(levelInfoFilePath))
        {
            if (line.StartsWith("width"))
            {
                levelWidth = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("height"))
            {
                levelHeight = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("seed"))
            {
                seed = int.Parse(line.Split(':')[1].Trim());
            }
        }
    }

    void SetUpLevelGrid()
    {
        string[] lines = File.ReadAllLines(levelInputFilePath);
        for(int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                levelGrid[x, y] = lines[y][x];
            }
        }
    }

    private int levelNumber;
    private string levelInputFilePath, levelInfoFilePath;
    [ContextMenu("Decorate level")]
    public void Decorate()
    {
        
        levelNumber = level_No;
        levelInputFilePath = "Assets/Generated/level" + levelNumber + ".txt";
        levelInfoFilePath = "Assets/Generated/level" + levelNumber + "info.txt";
        GetAttributes();
        UnityEngine.Random.InitState(seed);
        levelGrid = new char[levelWidth, levelHeight];
        SetUpLevelGrid();

        List<Vector2Int> dinobones = FindDinoBoneLocations();
        PlaceDinoBones(dinobones);
    }

    public List<Vector2Int> FindDinoBoneLocations()
    {
        String pattern = "#########";
        List<Vector2Int> dinobonelocations = new();
        for(int y = 1; y < levelHeight -1; ++y)
        {
            for(int x = 1; x < levelWidth -1; ++x)
            {
                if (levelGrid[x, y] == '#')
                {
                    if(GetPattern(x,y) == pattern)
                    {
                        dinobonelocations.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return dinobonelocations;
    }

    List<Vector2Int> Fisher_Yates(List<Vector2Int> orig)
    {
        for(int i = orig.Count -1; i > 0; --i)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (orig[i], orig[j]) = (orig[j], orig[i]);
        }
        return orig;
    }

    public void PlaceDinoBones(List<Vector2Int> locations)
    {
        var shuffledLoc = Fisher_Yates(locations);


    }



    string GetPattern(int x, int y)
    {
        StringBuilder sb = new();

        foreach (var d in patternCoordinates)
        {
            sb.Append(levelGrid[x + d.x, y + d.y]);
        }
        return sb.ToString();
    }




}


/**
 * ENCODINGS:
    0-a shroom cluster
    b-e single shroom
    f for embellishment tile
    g for skull in ground (rare)
    h for full dinosaur skeleton (big)
    i-q other dino bone elements in pallette
    and as previously # marks wall tile, * marks ground tile
 * 
 * 
 * 
 */
