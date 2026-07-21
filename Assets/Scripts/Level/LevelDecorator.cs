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

    // Take in only the grid data since the decoration generation is not based on the rules
    // alternatively for placing the dino tiles a sweep for tiles that can house the dino full skeleton needs to happen
    char[,] GeometryGrid; 
    char[,] DecorationsGrid;
    [SerializeField] int minDinoBones = 0;
    [SerializeField] int maxDinoBones = 3;
    [SerializeField] int ModifiedSeed = 0;
    int levelWidth, levelHeight, seed;
    List<Vector2Int> shroomSporeLocations;

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


            else if (line.StartsWith("seed"))
            {
                if (ModifiedSeed != 0)
                {
                    seed = ModifiedSeed;
                    return;
                }
                seed = int.Parse(line.Split(':')[1].Trim());
            }
        }
    }

    void SetUpLevelGrid()
    {
        string[] lines = File.ReadAllLines(GeometryFilePath);
        for(int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                GeometryGrid[x, y] = lines[y][x];
            }
        }
    }


    private string GeometryFilePath, LevelInfoFilePath, DecorationsFilePath;
    [ContextMenu("Decorate level")]
    public void Decorate()
    {

       
        string[] geo = Directory.GetFiles(
      "Assets/Generated/Working/",
      "*_geometry.txt");
        GeometryFilePath = geo[0];


        string levelPart = GeometryFilePath.Split("_")[0];
        int id = GeometryFilePath.IndexOf("_");
        int n = int.Parse(levelPart.Substring(id -3, 3));
       
        string levelString = $"level{n:D3}";
        DecorationsFilePath = "Assets/Generated/Working/" + levelString + "_decorations.txt";



        string[] info = Directory.GetFiles(
        "Assets/Generated/Working",
        "*_info.txt");
        LevelInfoFilePath = info[0];


        GetAttributes();
        UnityEngine.Random.InitState(seed);
        GeometryGrid = new char[levelWidth, levelHeight];
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

        // Idea : for example choose 10% of the good locations, then on success have a 50% chance to spawn a neighboring cluster
        // If fails, spawn single shroom
        foreach(Vector2Int pos in shroomSporeLocations)
        {
            DecorationsGrid[pos.x, pos.y] = 'C';
        }
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
        File.WriteAllText(DecorationsFilePath, output.ToString());
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
