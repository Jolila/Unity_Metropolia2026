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


    char[,] levelGrid; // Take in only the grid data since the decoration generation is not based on the rules





    [ContextMenu("Decorate level")]
    public void Decorate()
    {

        

        AddWallDecorations();
        AddDecorativeGroundTiles();
    }


    void AddDecorativeGroundTiles()
    {

    }

    void AddWallDecorations()
    {

    }




}


/**
 * ENCODINGS:
    0-a shroom cluster
    b-e single shroom
    f for embellishment tile
    g for skull in ground (rare)
    h for dino bones in walls (also rare)
    and as previously # marks wall tile, * marks ground tile
 * 
 * 
 * 
 */
