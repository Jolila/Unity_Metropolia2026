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


   

    [ContextMenu("Generate level")]
    public void Generate(){

        UnityEngine.Random.InitState(seed);
        string fileName = "Assets/Generated/level" + levelNumber + ".txt";
        char[,] levelGrid = new char[levelWidth, levelHeight];

        for(int y = 0; y < levelHeight; ++y)
        {
            for(int x = 0; x < levelWidth; ++x)
            {
                // size 2 for walls for the ruletilemap to look better
                if(y == 0 || y == levelHeight -2 ||
                    x == 0 || x == levelWidth -2){
                    levelGrid[x, y] = '#';
                }
                else {
                    levelGrid[x, y] = '*';
                }
                    
            }
        }
        generateIslets(levelGrid);
        OutputLevel(fileName, levelGrid);
    }

    void generateIslets(char[,] levelGrid)
    {

        int isletsPlaced = 0;
        int iterations = 0;
        /*
         * x ranging from 3+islet_width to _width -1 -islet_Width (+3 to disallow non-passable path between wall edge and islet edge)
         * y ranging from 3+islet_height to height -1 -isletHeight (+3 can be made to a serializable if desired)
         * generate one random x and y
         * generate additional islets by validating the minimum distance
         * iterate through max iterations or until the islets are placed
         */
        //int firstIsletY = UnityEngine.Random.Range(3 + isletHeight, levelHeight - 1 - isletHeight);
        //int firstIsletX = UnityEngine.Random.Range(3 + isletWidth, levelWidth - 1 - isletWidth);

        //Debug.Log("X : " + firstIsletX + " , Y :" + firstIsletY);

        //List<Vector2Int> isletPositions = new List<Vector2Int>();
        //isletPositions.Add(new Vector2Int(firstIsletX, firstIsletY));

        //for(int j = firstIsletY; j < firstIsletY + isletHeight; ++j){
        //    for(int i = firstIsletX; i < firstIsletX + isletWidth; ++i)
        //    {
        //        levelGrid[i, j] = '#';
        //    }
        //}
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

        File.WriteAllText(filePath, output.ToString());
      
    }



}