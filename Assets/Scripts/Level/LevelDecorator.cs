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


    // Fetch walls ruletile:

    string WallRuleTilePath = "Assets/Generated/WallRuleTile.txt";
    string[] rules; // for filtering wall rule decorations

    string NormalizeRule(string rule)
    {
        return rule
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "")
            .Replace('C', '#');
    }

    [ContextMenu("Decorate level")]
    public void Decorate()
    {

        string text = File.ReadAllText(WallRuleTilePath);
        string[] mRules = text.Split(
     new[] { "\r\n\r\n", "\n\n" },
     StringSplitOptions.RemoveEmptyEntries);

        rules = mRules.Select(NormalizeRule).ToArray();


        AddDecorativeGroundTiles();
    }


    void AddDecorativeGroundTiles()
    {

    }


}
