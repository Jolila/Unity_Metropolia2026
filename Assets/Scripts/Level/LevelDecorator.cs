using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
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

    private static readonly Vector2Int[] neighborDirections =
{
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1,1),
        };

    // Take in only the grid data since the decoration generation is not based on the rules
    // alternatively for placing the dino tiles a sweep for tiles that can house the dino full skeleton needs to happen
    char[,] GeometryGrid; 
    char[,] DecorationsGrid;
    [SerializeField] int minDinoBones = 0;
    [SerializeField] int maxDinoBones = 5;
    [SerializeField] int MinimumDistanceBetweenDinoBones = 3;
    [SerializeField] int ModifiedSeed = 0;
    [SerializeField] int MaxShroomPlacementIterations = 4;
    [SerializeField] float ShroomClusterDensityWeight = 0.8f;
    [SerializeField] float SingleShroomDensityWeight = 0.1f;


    

    [SerializeField] float MinimumShroomSpawnPropability = 0.0f;
    [SerializeField] float MaximumShroomSpawnPropability = 0.7f;

    [SerializeField] float InitialShroomSporeSpawnChance = 0.2f;

    [SerializeField] float ShroomClusterDensityThreshhold = 0.6f;
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
        PlaceSkull();
        OutputLevelDecorations();
        AssetDatabase.Refresh();
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

    private bool IsTooClose(Vector2Int a, Vector2Int b, int minimumDistance)
    {
        float epsi = 0.001f;
        int x1 = a.x;
        int x2 = b.x;
        double dx = (x1 - x2) * (x1 - x2);
        double distx = Math.Sqrt(dx);
        if (Math.Abs(distx - minimumDistance) <= epsi) return true;
        

        int y1 = a.y;
        int y2 = b.y;
        double dy = (y1 - y2) * (y1 - y2);
        double disty = Math.Sqrt(dy);
        if (Math.Abs(disty - minimumDistance) <= epsi) return true;
        return false;
        
    }

    public void PlaceDinoBones(List<Vector2Int> locations)
    {

        int n = UnityEngine.Random.Range(0, maxDinoBones + 1);
        var shuffledLoc = Fisher_Yates(locations);
        var used = new List<Vector2Int>();
        int max = shuffledLoc.Count < n ? shuffledLoc.Count : n;

        for(int i = 0; i < max; ++i)
        {

            //check if too close
            foreach(Vector2Int other in used)
            {
                if (IsTooClose(shuffledLoc[i], other, MinimumDistanceBetweenDinoBones))
                {
                    ++max;
                    continue;
                }
            }

            // roll for the full skeleton spawn, h
            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.95)
            {
                DecorationsGrid[shuffledLoc[i].x, shuffledLoc[i].y] = 'h';
            }
            // else : output d for random dino bone tile
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
        

        // place seed shrooms
        foreach(Vector2Int pos in shroomSporeLocations)
        {
            float roll = UnityEngine.Random.Range(0.0f, 1.0f);
            if(roll > 1.0f - InitialShroomSporeSpawnChance) DecorationsGrid[pos.x, pos.y] = 'C';
        }

        // place additional clusters and single shrooms based on densities and cluster spawning threshold
        int shroomIteration = 0;
        while(shroomIteration <= MaxShroomPlacementIterations)
        {
            
            for (int y = 1; y < levelHeight - 1; ++y)
            {
                for (int x = 1; x < levelWidth - 1; ++x)
                {
                    if (DecorationsGrid[x, y] == 'C' || DecorationsGrid[x,y] == 'S')
                    {
                        float density = EvaluateShroomDensity(new Vector2Int(x, y));
                        float p = Mathf.Lerp(MinimumShroomSpawnPropability, MaximumShroomSpawnPropability, density);
                        float seededPropability = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (seededPropability > p)
                        {
                            if (density > ShroomClusterDensityThreshhold)
                            {

                                DecorationsGrid[x, y] = 'C';
                            }
                            else
                            {
                                DecorationsGrid[x, y] = 'S';
                            }
                        }

                    }
                }
            }
            ++shroomIteration;
        }

        
    }


  
    // remember to normalize the density by making the returned local density to correspond exaclty 1/8 portion of the host shrooms density.
    // ie this makes this density a 
    private float EvaluateShroomDensity(Vector2Int pos)
    {
        int clustercount = 0, singlescount = 0;

        foreach (Vector2Int v in neighborDirections)
        {
            Vector2Int neighbor = new Vector2Int(pos.x + v.x, pos.y + v.y);
            if (DecorationsGrid[pos.x + v.x, pos.y + v.y] == 'C') clustercount++;
            else if (DecorationsGrid[((Vector3Int)pos).x, pos.y + v.y] == 'S') singlescount++;
        }
     
        return (clustercount * ShroomClusterDensityWeight 
            + singlescount * SingleShroomDensityWeight) / 8.0f;

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

    void PlaceSkull()
    {

        bool skullPlaced = false;
        while(!skullPlaced)
        {

        
        for(int y = 2; y < levelHeight - 2; ++y)
        {
            for(int x = 2; x < levelWidth -2; ++x)
            {
                    if (GeometryGrid[x,y] == '*')
                    {
                        float f = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (f > 0.99)
                        {
                            DecorationsGrid[x, y] = 'X';
                            skullPlaced = true;
                            return;
                        }
                    }
            }
        }

        }
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
    d other dino bone elements in pallette, lets try this for now
    X for skull in ground (rare)
    h for full dinosaur skeleton (big)
    and as previously # marks wall tile, * marks ground tile
 */
