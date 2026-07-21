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

    char[,] GeometryGrid; // Take in only the grid data since the decoration generation is not based on the rules
    // alternatively for placing the dino tiles a sweep for tiles that can house the dino full skeleton needs to happen

    char[,] DecorationsGrid;


    [SerializeField] int level_No = 0;

    [SerializeField] int minDinoBones = 0;
    [SerializeField] int maxDinoBones = 3;
    int levelWidth, levelHeight, seed;
    List<Vector2Int> shroomSporeLocations;

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
                GeometryGrid[x, y] = lines[y][x];
            }
        }
    }

    private int levelNumber;
    private string levelInputFilePath, levelInfoFilePath, outputFilePath;
    [ContextMenu("Decorate level")]
    public void Decorate()
    {
        
        levelNumber = level_No;
        levelInputFilePath = "Assets/Generated/Working/level" + levelNumber + "_geometry.txt";
        levelInfoFilePath = "Assets/Generated/Working/level" + levelNumber + "_info.txt";
        outputFilePath = "Assets/Generated/Working/level" + levelNumber + "_decorations.txt";
        GetAttributes();
        UnityEngine.Random.InitState(seed);
        DecorationsGrid = new char[levelWidth, levelHeight];
        SetUpLevelGrid();

        List<Vector2Int> dinobones = FindDinoBoneLocations();
        PlaceDinoBones(dinobones);
        PlaceShrooms();
        OutputLevelDecorations();
    }

    public List<Vector2Int> FindDinoBoneLocations()
    {
        String pattern = "#########";
        List<Vector2Int> dinobonelocations = new();
        for(int y = 1; y < levelHeight -1; ++y)
        {
            for(int x = 1; x < levelWidth -1; ++x)
            {
                if (GeometryGrid[x, y] == '#')
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

        int max = shuffledLoc.Count < maxDinoBones ? shuffledLoc.Count : maxDinoBones;
        string dinostring = "ijklmnopqr"; // 10 as in 10 total more common dino bone fragments
        for(int i = 0; i < max; ++i)
        {
            // roll for the full skeleton spawn, h
            // else : output i-q
            if(UnityEngine.Random.Range(0.0f, 1.0f) > 0.95)
            {
                DecorationsGrid[shuffledLoc[i].x, shuffledLoc[i].y] = 'h';
            }
            else
            {
                int p = UnityEngine.Random.Range(0, dinostring.Length);
                char c = dinostring[p];
                DecorationsGrid[shuffledLoc[i].x, shuffledLoc[i].y] = 'd';
            }
        }

        int remaining = shuffledLoc.Count - maxDinoBones;
        if (remaining < 0) return;

        for(int i = max; i < max + remaining; ++i)
        {
            shroomSporeLocations.Add(shuffledLoc[i]);
        }

    }

    public void PlaceShrooms()
    {
        // TO DO : try to evaluate shroom densities by using a propability based approach? (:
    }



    string GetPattern(int x, int y)
    {
        StringBuilder sb = new();

        foreach (var d in patternCoordinates)
        {
            sb.Append(GeometryGrid[x + d.x, y + d.y]);
        }
        return sb.ToString();
    }



    void OutputLevelDecorations()
    {
        StringBuilder output = new();
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                output.Append(DecorationsGrid[x, y]);
            }
            output.AppendLine();
        }
        File.WriteAllText(outputFilePath, output.ToString());
    }


}


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
