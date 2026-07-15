using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Text;
using NUnit.Framework;
using System.Linq;
using JetBrains.Annotations;

public class LevelGenerator : MonoBehaviour
{



    [SerializeField] int levelWidth = 64;
    [SerializeField] int levelHeight = 64;
    [SerializeField] int seed = 1337;
    [SerializeField] int levelNumber = 0;
    [SerializeField] int numOfIslets = 2;
    [SerializeField] int isletHeight = 2;
    [SerializeField] int isletWidth = 3;
    [SerializeField] int spaceBetweenIslets = 2;
    [SerializeField] int maxIterations = 100;
    [SerializeField] int protrusionRollMax = 255;
    [SerializeField] int protrusionRollThreshold = 140;


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
        generateProtrusions(levelGrid);
        OutputLevel(fileName, levelGrid);
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
        int protrusionX = UnityEngine.Random.Range(0, 3);
        int protrusionY = UnityEngine.Random.Range(0, 3);
        protrusionY = 4; // rolling 0 for testing is fun
        int roll = UnityEngine.Random.Range(0, protrusionRollMax);

        for (int y = 2; y < levelHeight - 2; ++y)
        {
            for (int x = 2; x < levelWidth - 2; ++x)
            {

                if (roll > protrusionRollThreshold)
                {

                    // check for neighboring '#' tiles
                    Vector2Int pos = new Vector2Int(x, y);

                    int neighborCount = 0;
                    foreach (Vector2Int v in neighborDirections)
                    {
                        if (levelGrid[x + v.x, y + v.y] == '#')
                        {

                            neighborCount++;
                            break;
                        }
                    }

                    if (neighborCount < 1) continue;
                    for (int proty = y; proty < y + protrusionY; ++proty)
                    {
                        for (int protx = x; protx < x + protrusionX; ++protx)
                        {
                            levelGrid[protx, proty] = '#';
                        }
                    }
                    protrusionX = UnityEngine.Random.Range(0, 3);
                    protrusionY = UnityEngine.Random.Range(0, 3);
                }

                roll = UnityEngine.Random.Range(0, protrusionRollMax);
                Debug.Log("Rolled : " + roll);
            }

        }
    }
    /*
    * x ranging from 3+islet_width to _width -1 -islet_Width (+3 to disallow non-passable path between wall edge and islet edge)
    * y ranging from 3+islet_height to height -1 -isletHeight (+3 can be made to a serializable if desired)
    * generate one random x and y
    * generate additional islets by validating the minimum distance
    * iterate through max iterations or until the islets are placed
    */
    void generateIslets(char[,] levelGrid)
    {

        int isletsPlaced = 0;
        int iterations = 0;

        int x, y;
        float epsi = 0.01f;
        List<Vector2Int> isletPositions = new List<Vector2Int>();
        while (isletsPlaced != numOfIslets && iterations < maxIterations)
        {

            // generate new x, new y
            // loop through the existing islet locations, and try to get a non-clashing x and y
            x = UnityEngine.Random.Range(3 + isletHeight, levelHeight - 1 - isletHeight);
            y = UnityEngine.Random.Range(3 + isletWidth, levelWidth - 1 - isletWidth);

            // if the distance between x + width and pos.x - width is less than space between, reroll for new x
            // repeat for y
            bool isClash = false;
            foreach(Vector2Int pos in isletPositions){
                int x1 = x + isletWidth;
                int x2 = pos.x + isletWidth;
                double dx = (x1 - x2) * (x1 - x2);
                double distx = Math.Sqrt(dx);
                if (Math.Abs(distx - spaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }

                int y1 = y + isletHeight;
                int y2 = pos.y + isletHeight;
                double dy = (y1 - y2) * (y1 - y2);
                double disty = Math.Sqrt(dy);
                if (Math.Abs(disty - spaceBetweenIslets) <= epsi){
                    isClash = true;
                    break;
                }
            }
            if(!isClash){
                for (int j = x; j < x + isletHeight; ++j)
                {
                    for (int i = y; i < y + isletWidth; ++i)
                    {
                        levelGrid[i, j] = '#';
                    }
                }
            }
            isletPositions.Add(new Vector2Int(x, y));
            ++isletsPlaced;
            ++iterations;

        }
        Debug.Log("Iterations used " + iterations + ", produced :" + isletsPlaced + " islets");
      
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


        File.WriteAllText(filePath, output.ToString());
        
        
    }



}