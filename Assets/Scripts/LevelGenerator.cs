using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using UnityEngine.Tilemaps;
using NUnit.Framework;
using System.Linq;
using JetBrains.Annotations;

public class LevelGenerator : MonoBehaviour
{

    struct Islet
    {
        public int height, width;
        public Vector2Int pos;
        public Islet(int h, int w, Vector2Int position)
        {
            height = h;
            width = w;
            pos = position;
        }
    }
    /*
     * levelWidth : 64
levelHeight : 64
seed : 42
num of islets : 15
minIsletHeight : 2
maxIsletHeight : 9
minIsletWidth : 2
minIsletWidth : 9
space between islets : 3
maxIterations : 100
protrusionrollmax : 255
protrusionrollThreshold: 140
maxYProtrusion : 3
maxXProtrusion : 3
     * 
     * */

    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelHeight = 64;
    [SerializeField] int seed = 42;
    [SerializeField] int levelNumber = 0;
    [SerializeField] int numOfIslets = 2;
    [SerializeField] int minIsletHeight = 2;
    [SerializeField] int maxIsletHeight = 9;
    [SerializeField] int minIsletWidth = 2;
    [SerializeField] int maxIsletWidth = 9;
    [SerializeField] int spaceBetweenIslets = 3;
    [SerializeField] int maxIterations = 100;
    [SerializeField] int protrusionRollMax = 255;
    [SerializeField] int protrusionRollThreshold = 64;
    [SerializeField] int maxXProtrusion = 5;
    [SerializeField] int maxYProtrusion = 5;
    [SerializeField] bool protuse = true;

    [SerializeField] Tilemap groundTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap decorationsTilemap;
    [SerializeField] Tilemap outlineTileMap;

    [SerializeField] RuleTile groundTile;
    [SerializeField] RuleTile wallTile;


    [ContextMenu("Clear tilemaps")]
    public void ClearTileMaps(){

        groundTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        decorationsTilemap.ClearAllTiles();
        outlineTileMap.ClearAllTiles();

    }


    private static readonly Vector2Int[] neighborDirections =
    {
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1,1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        };

    private static readonly Vector2Int[] cardinalNeighbors =
    {
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0)
    };

    private static readonly Vector2Int[] diagonalNeighbors =
    {
        new Vector2Int(1,1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
    };


[ContextMenu("Generate level")]
    public void Generate(){

        UnityEngine.Random.InitState(seed);
        string fileName = "Assets/Generated/level" + levelNumber + ".txt";
        char[,] levelGrid = new char[levelWidth, levelHeight];

        // Initialize all stars for starters
        for(int y = 0; y < levelHeight; ++y){
            for(int x = 0; x < levelWidth; ++x){
                levelGrid[x, y] = '*';
            }
        }

        generateWalls(levelGrid);
        generateIslets(levelGrid);
        if (protuse) {
            generateProtrusions(levelGrid);
        }
        
        repairWalls(levelGrid);
        OutputLevel(fileName, levelGrid);
        RenderLevel(levelGrid);
    }

    void generateWalls(char[,] levelGrid)
    {
        // default walls : length and witdh of two encompassing the level
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                if (y < 2 || y + 2 >= levelHeight ||
                   x < 2 || x + 2 >= levelWidth)
                {
                    levelGrid[x, y] = '#';
                }
            }
        }

    }
    /* Iterate through the array
 * roll for a chance to create protrusion on a tile that has a neighboring #
 * roll again for the type of protrusion, then roll and loop for how much protrusion there is
 * (alternatively, create protrusion in both direction once the core loop works?)
 */
    void generateProtrusions(char[,] levelGrid)
    {
        int protrusionX = UnityEngine.Random.Range(2, maxXProtrusion);
        int protrusionY = UnityEngine.Random.Range(2, maxYProtrusion);
        int roll = UnityEngine.Random.Range(0, protrusionRollMax);
        int totalProtrusions = 0;

        for (int y = 2; y < levelHeight - 2; ++y)
        {
            for (int x = 2; x < levelWidth - 2; ++x)
            {

                if (roll > protrusionRollThreshold)
                {

                    // check for neighboring '#' tiles
                    Vector2Int pos = new Vector2Int(x, y);

                    int neighborCount = 0;
                    List<Vector2Int> non_wall_neighbors = new();
                    foreach (Vector2Int v in neighborDirections)
                    {
                        if (levelGrid[x + v.x, y + v.y] == '#')
                        {
                            neighborCount++;
                            
                        }
                        else
                        {
                            non_wall_neighbors.Add(v);
                        }
                    }

                    if (neighborCount < 1) continue;
                    // UPDATE : Find suitable direction for protrusion.
                    

                    for (int proty = y; proty < y + protrusionY; ++proty)
                        {
                            for (int protx = x; protx < protrusionX; ++protx)
                            {
                                levelGrid[protx, proty] = '#';
                            }
                        }


                    protrusionX = UnityEngine.Random.Range(2, maxXProtrusion);
                    protrusionY = UnityEngine.Random.Range(2, maxYProtrusion);
                    ++totalProtrusions;
                }

                roll = UnityEngine.Random.Range(0, protrusionRollMax);
                
            }

        }
        Debug.Log("Succesful protrusions: " + totalProtrusions +" times ");
    }
    /*
    * x ranging from 3+islet_width to _width -1 -islet_Width (+3 to disallow non-passable path between wall edge and islet edge)
    * y ranging from 3+islet_height to height -1 -isletHeight (+3 can be made to a serializable if desired)
    * generate one random x and y
    * generate additional islets by validating the minimum distance
    * iterate through max iterations or until the islets are placed

    NOTE : The theorycrafing up did not lead to a fruitful resolution, and likely an additional sweep needs to be done to preserve the tilemap constraints.
    */
    void generateIslets(char[,] levelGrid)
    {

        int isletsPlaced = 0;
        int iterations = 0;
        List<Islet> islets = new List<Islet>();

        int x, y;
        float epsi = 0.01f;
        List<Vector2Int> isletPositions = new List<Vector2Int>();


        while (isletsPlaced <= numOfIslets && iterations < maxIterations)
        {

            // generate new x, new y
            // loop through the existing islet locations, and try to get a non-clashing x and y
            int isletHeight = UnityEngine.Random.Range(minIsletHeight, maxIsletHeight);
            int isletWidth = UnityEngine.Random.Range(minIsletWidth, maxIsletWidth);

            x = UnityEngine.Random.Range(3 + isletHeight, levelHeight - 1 - isletHeight);
            y = UnityEngine.Random.Range(3 + isletWidth, levelWidth - 1 - isletWidth);

            // if the distance between x + width and pos.x - width is less than space between, reroll for new x
            // repeat for y
            bool isClash = false;
            foreach(Islet other in islets){
                int x1 = x + isletWidth;
                int x2 = other.pos.x + other.width;
                double dx = (x1 - x2) * (x1 - x2);
                double distx = Math.Sqrt(dx);
                if (Math.Abs(distx - spaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }

                int y1 = y + isletHeight;
                int y2 = other.pos.y + other.height;
                double dy = (y1 - y2) * (y1 - y2);
                double disty = Math.Sqrt(dy);
                if (Math.Abs(disty - spaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }
            }
            if(!isClash){
                for (int j = y; j < y + isletHeight; ++j)
                {
                    for (int i = x; i < x + isletWidth; ++i)
                    {
                        levelGrid[i, j] = '#';
                    }
                }
                islets.Add(new Islet(isletHeight, isletWidth, new Vector2Int(x, y)));
                ++isletsPlaced;
                ++iterations;
            }
            

        }
        Debug.Log("Iterations used " + iterations + ", produced :" + isletsPlaced + " islets");
      
    }

    string[] legalPatterns =
    {
@"###
#C#
##X",

@"###
#C#
X##",

@"_#_
#C#
_X_",

@"_##
XC#
_##",

@"##X
#C#
###",

@"X##
#C#
###",

@"_X_
#C#
###",

@"##_
#CX
##_",

@"_X_
XC#
_##",

@"_X_
#CX
##_",

@"_##
XC#
_X_",

@"##_
#CX
_X_",

@"###
#C#
###"
};



    void repairWalls(char[,]levelGrid)
    {

        

    }

    void OutputLevel(string filePath, char[,] levelGrid)
    {
        StringBuilder output = new StringBuilder();
        
        for (int y = 0; y < levelWidth; ++y)
        {
            for (int x = 0; x < levelHeight; ++x)
            {
                output.Append(levelGrid[x, y]);
            }
            output.AppendLine();
        }
        Debug.Log(output);

        output.Append("Config : ");
 

        output.Append("levelWidth : " + levelWidth + "\n");
        output.Append("levelHeight : " + levelHeight + "\n");
        output.Append("seed : " + seed + "\n");
        output.Append("num of islets : " + numOfIslets+ "\n");
        output.Append("minIsletHeight : " + minIsletHeight + "\n");
        output.Append("maxIsletHeight : " + maxIsletHeight + "\n");
        output.Append("minIsletWidth : " + minIsletWidth + "\n");
        output.Append("minIsletWidth : " + maxIsletWidth + "\n");

        output.Append("space between islets : " + spaceBetweenIslets + "\n");
        output.Append("maxIterations : " + maxIterations + "\n");
        output.Append("protrusionrollmax : " + protrusionRollMax+ "\n");
        output.Append("protrusionrollThreshold: " + protrusionRollThreshold+ "\n");
        output.Append("maxYProtrusion : " + maxYProtrusion + "\n");
        output.Append("maxXProtrusion : " + maxYProtrusion+ "\n");
        File.WriteAllText(filePath, output.ToString());
        
   
    }

    void RenderLevel(char[,] levelGrid)
    {
        ClearTileMaps();
        for(int y = 0; y < levelHeight; ++y)
        {
            for(int x = 0; x < levelWidth; ++x)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (levelGrid[x,y] == '#')
                {
                    wallTilemap.SetTile(cell, wallTile);
                }
                else
                {
                    groundTilemap.SetTile(cell, groundTile);
                }
            }
        }


    }



}