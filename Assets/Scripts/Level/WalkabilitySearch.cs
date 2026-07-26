using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkabilitySearch : MonoBehaviour
{



    /* What this class needs :
    - find the player starting position from level geometry
    - perform a search form that tile into all walkable tiles. Put all walkable tiles in set/map
    - then, mark every non-walkable but legit wall formation with a debug sentinel value
    - replace sentinel values with a wall tiles, that is edit both the ground and the wall tilemap
    */

    string GeometryFilePath, LevelInfoFilePath;
    int levelWidth, levelHeight, playerX, playerY;
    char[,] GeometryGrid;

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
            else if(line.StartsWith("playerX"))
            {
                playerX = int.Parse(line.Split(':')[1].Trim());
                Debug.Log("player X position : " + playerX);
            }
            else if(line.StartsWith("playerY"))
            {
                playerY = int.Parse(line.Split(':')[1].Trim());
                Debug.Log("player Y position : " + playerY);
            }
        }
    }

    private static readonly Vector2Int[] neighborDirections =
{
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        };


    void SetUpLevelGrid()
    {
        string[] lines = File.ReadAllLines(GeometryFilePath);
        Debug.Log("how many lines in lines when reading file : " + lines.Count());
        Debug.Log("Line width when reading file : " + lines[0].Length);
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                GeometryGrid[x, y] = lines[y][x];
            }
        }
    }


    [ContextMenu("Search & Repair non-walkable walls")]
    public void RepairNonWalkable()
    {
        string[] geo = Directory.GetFiles(
       "Assets/Generated/Working",
       "*_geometry.txt");
        GeometryFilePath = geo[0];

        string[] info = Directory.GetFiles(
        "Assets/Generated/Working",
        "*_info.txt");
        LevelInfoFilePath = info[0];

        GetAttributes();
        GeometryGrid = new char[levelWidth, levelHeight];
        SetUpLevelGrid();
        List<Vector2Int> bads = FindNonWalkableTiles(FindWalkableTiles());
        OutputNonWalkables(bads);
        OutputLevel();
    }


    public HashSet<Vector2Int> FindWalkableTiles()
    {


        // initialize a set called walkable
        // start the player from x,y
        // initialize a queue called current
        // loop exit condition is : if the length of the set is the same as previous iteration and the queue is empty twice in a row

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Vector2Int startPos = new Vector2Int(playerX, playerY);

        queue.Enqueue(startPos);
        visited.Add(startPos);
        

        while(queue.Count != 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach(Vector2Int cardinalNeighbor in neighborDirections)
            {
                Vector2Int p = current + cardinalNeighbor;

                if (GeometryGrid[p.x,p.y] == '*' && visited.Add(p))
                {
                    queue.Enqueue(p);
                }
                
            }
            
        }


        return visited;
    }

    public List<Vector2Int> FindNonWalkableTiles(HashSet<Vector2Int> walkables)
    {
        List<Vector2Int> nonWalkables = new();
        Vector2Int p;
        for(int y = 0; y < levelHeight; ++y)
        {
            for(int x = 0; x < levelWidth; ++x)
            {
                if (GeometryGrid[x,y] == '*')
                {
                    p = new Vector2Int(x, y);
                    if (!(walkables.Contains(p))) nonWalkables.Add(p);
                }
            }
        }

        return nonWalkables;
    }

    public void OutputNonWalkables(List<Vector2Int> nonWalkables)
    {
    
        foreach (Vector2Int v in nonWalkables)
        {
            
            GeometryGrid[v.x, v.y] = 'N';
            Debug.Log("Found non-walkable tile: ("+  v.x + "," + v.y + ")");
        }
    }


    void OutputLevel()
    {
        StringBuilder output = new();
        for (int y = 0; y < levelHeight; ++y)
        {
            for (int x = 0; x < levelWidth; ++x)
            {
                output.Append(GeometryGrid[x, y]);
            }
            output.AppendLine();
        }
        File.WriteAllText(GeometryFilePath, output.ToString());

    }

}
