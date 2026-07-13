using System.Collections;
using System.IO;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    int levelWidth = 16;
    int levelHeight = 16;

    private char[,] levelGrid = new char[16, 16];

    [ContextMenu("Generate level")]
    public void Generate()
    {
  

        for(int x = 0; x < levelWidth; ++x)
        {
            for(int y = 0; y < levelHeight; ++y)
            {
                
                levelGrid[x, y] = '*';
            }
        }
        OutputLevel();
    }

    void OutputLevel()
    {
        string output = "";
        for (int x = 0; x < levelWidth; ++x)
        {
            for (int y = 0; y < levelHeight; ++y)
            {
                output += levelGrid[x, y];
            }
            output += "\n";
        }

        Debug.Log(output);
    }


}
