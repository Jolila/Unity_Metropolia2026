using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    char[,] OutlineGrid;
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
                Debug.Log("Parsed width : " + levelWidth);
            }
            else if (line.StartsWith("height"))
            {
                levelHeight = int.Parse(line.Split(':')[1].Trim());
                Debug.Log("Parsed height : " + levelHeight);
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
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                GeometryGrid[x, y] = lines[y][x];
            }
        }
    }


    private string GeometryFilePath, LevelInfoFilePath, DecorationsFilePath, OutLineFilePath;
    private int OutlinePadding = 10;
    int outlineHeight, outlineWidth;
    [ContextMenu("Decorate level")]
    public void Decorate()
    {


        string[] geo = Directory.GetFiles(
      "Assets/Generated/Working/",
      "*_geometry.txt");
        GeometryFilePath = geo[0];


        string levelPart = GeometryFilePath.Split("_")[0];
        int id = GeometryFilePath.IndexOf("_");
        int n = int.Parse(levelPart.Substring(id - 3, 3));

        string levelString = $"level{n:D3}";
        DecorationsFilePath = "Assets/Generated/Working/" + levelString + "_decorations.txt";
        OutLineFilePath = "Assets/Generated/Working/" + levelString + "_outline.txt";



        string[] info = Directory.GetFiles(
        "Assets/Generated/Working",
        "*_info.txt");
        LevelInfoFilePath = info[0];


        GetAttributes();
        UnityEngine.Random.InitState(seed);
        GeometryGrid = new char[levelWidth, levelHeight];
        DecorationsGrid = new char[levelWidth, levelHeight];
        SetUpLevelGrid();
        //Debug.Log("Decorations width " + DecorationsGrid.Length / levelHeight);
        //Debug.Log("Decorations height " + DecorationsGrid.Length / levelWidth);
        shroomSporeLocations = new();

        List<Vector2Int> dinobones = FindDinoBoneLocations();
        PlaceDinoBones(dinobones);
        PlaceShrooms(DecorationsGrid, levelWidth, levelHeight, shroomSporeLocations);
        PlaceSkull();
        
        //AssetDatabase.Refresh();
        GenerateOutline();
        PlaceShrooms(OutlineGrid, outlineWidth, outlineHeight, FindOutlineLocations(OutlineGrid, outlineWidth, outlineHeight));
        OutputLevelOutline();

        OutputLevelDecorations();
    }


    public void GenerateOutline()
    {
        outlineWidth = levelWidth + OutlinePadding * 2;
        outlineHeight = levelHeight + OutlinePadding * 2;
        OutlineGrid = new char[outlineWidth, outlineHeight];
   

        for (int y = 0; y < outlineHeight; y++)
        {
            for (int x = 0; x < outlineWidth; x++)
            {
                OutlineGrid[x, y] = ' ';
            }
        }

        // Top & Bottom strips.
        for (int x = 0; x < outlineWidth; x++)
        {
            for (int y = 0; y < OutlinePadding; y++)
            {
                OutlineGrid[x, y] = 'O';
                OutlineGrid[x, outlineHeight - 1 - y] = 'O';
            }
        }

        // Left & Right strips.
        for (int y = OutlinePadding; y < outlineHeight - OutlinePadding; y++)
        {
            for (int x = 0; x < OutlinePadding; x++)
            {
                OutlineGrid[x, y] = 'O';
                OutlineGrid[outlineWidth - 1 - x, y] = 'O';
            }
        }

    }

    public List<Vector2Int> FindOutlineLocations(char[,] outlineGrid, int width, int height)
    {
        List<Vector2Int> locations = new();

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (outlineGrid[x, y] == 'O')
                {
                    locations.Add(new Vector2Int(x, y));
                }
            }
        }

        return locations;
    }

    public List<Vector2Int> FindDinoBoneLocations()
    {
        String pattern = "#########";
        List<Vector2Int> dinobonelocations = new();
        for (int y = 1; y < levelHeight - 1; ++y)
        {
            for (int x = 1; x < levelWidth - 1; ++x)
            {
                if (GeometryGrid[x, y] == '#')
                {
                    if (GetPattern(x, y) == pattern)
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
        for (int i = orig.Count - 1; i > 0; --i)
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
    // This might be too greedy ? 
    public void PlaceDinoBones(List<Vector2Int> locations)
    {

        int n = UnityEngine.Random.Range(0, maxDinoBones + 1);
        var shuffledLoc = Fisher_Yates(locations);
        var used = new List<Vector2Int>();
        int max = shuffledLoc.Count < n ? shuffledLoc.Count : n;

        for (int i = 0; i < max; ++i)
        {

            //check if too close
            foreach (Vector2Int other in used)
            {
                if (IsTooClose(shuffledLoc[i], other, MinimumDistanceBetweenDinoBones))
                {
                    --i;
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

        int remaining = shuffledLoc.Count - max;
        if (remaining < 0) return;

        for (int i = max; i < max + remaining; ++i)
        {
            shroomSporeLocations.Add(shuffledLoc[i]);
        }

    }

    public void PlaceShrooms(char[,] grid, int width, int height, List<Vector2Int> seedLocations)
    {


        // place seed shrooms
        foreach (Vector2Int pos in seedLocations)
        {
     
            float roll = UnityEngine.Random.Range(0.0f, 1.0f);
            // is the dino bone search simply too greedy for trying to place the bones far enough from each other?
            if (roll > 1.0f - InitialShroomSporeSpawnChance) grid[pos.x, pos.y] = 'C';
        }

        // place additional clusters and single shrooms based on densities and cluster spawning threshold
        int shroomIteration = 0;
        while (shroomIteration <= MaxShroomPlacementIterations)
        {

            for (int y = 1; y < height - 1; ++y)
            {
                for (int x = 1; x < width - 1; ++x)
                {
                    if (grid[x, y] == 'C' || grid[x, y] == 'S')
                    {
                        float density = EvaluateShroomDensity(grid,new Vector2Int(x, y), width, height);
                        float p = Mathf.Lerp(MinimumShroomSpawnPropability, MaximumShroomSpawnPropability, density);
                        float seededPropability = UnityEngine.Random.Range(0.0f, 1.0f);
                        if (seededPropability > p)
                        {
                            if (density > ShroomClusterDensityThreshhold)
                            {

                                grid[x, y] = 'C';
                            }
                            else
                            {
                                grid[x, y] = 'S';
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
    private float EvaluateShroomDensity(char[,] grid,Vector2Int pos, int width, int height)
    {
        int clustercount = 0, singlescount = 0;

        foreach (Vector2Int v in neighborDirections)
        {
            Vector2Int neighbor = new Vector2Int(pos.x + v.x, pos.y + v.y);
            if (grid[pos.x + v.x, pos.y + v.y] == 'C') clustercount++;
            else if (grid[((Vector3Int)pos).x, pos.y + v.y] == 'S') singlescount++;
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
        while (!skullPlaced)
        {


            for (int y = 2; y < levelHeight - 2; ++y)
            {
                for (int x = 2; x < levelWidth - 2; ++x)
                {
                    if (GeometryGrid[x, y] == '*')
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




    void OutputLevelOutline()
    {
        StringBuilder output = new();

        for (int y = outlineHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < outlineWidth; x++)
            {
                output.Append(OutlineGrid[x, y]);
            }
            output.AppendLine();
            
        }
        File.WriteAllText(OutLineFilePath, output.ToString());
        
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
