using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


/**
 * A simple helper class which enables semi-automatic conversion from a unity ruletile set into something that the level generator
 * can use in validating the generated tilemap.
 * 
 */
public class RuleTileExporter : MonoBehaviour
{

    [SerializeField] RuleTile Rules;

    /**
     * 
     * EDIT : Due to previous work,
     * Now that we know we can access all the members quite easily since they are public, the method becomes quite straightforward.
     */
    [ContextMenu("Export Rule Tileset")]
    public void ExportRuleset()
    {
        string filepath = "Assets/Scripts/Level" + Rules.name + ".txt";
        List<string> rules = new();
        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Public;

        Debug.Log($"Rules :  {Rules.m_TilingRules.Count}");

        foreach (var rule in Rules.m_TilingRules)
        {
            var neighbors = (List<int>)
                typeof(RuleTile.TilingRule)
                .GetField("m_Neighbors", flags)
                .GetValue(rule);

            var positions = (List<Vector3Int>)
                typeof(RuleTile.TilingRule)
                .GetField("m_NeighborPositions", flags)
                .GetValue(rule);

            Debug.Log($"Rule {rule.m_Id}");

            Dictionary<Vector3Int, int> numbermap = new();

            for (int i = 0; i < neighbors.Count; ++i)
            {
                Debug.Log($"{positions[i]} -> {neighbors[i]}");
                numbermap.Add(positions[i], neighbors[i]);
            }

           rules.Add(generatePattern(numbermap));

           
        }

        File.WriteAllText(filepath, string.Join("\n\n", rules)); // extra newline between patterns for inspecting the file


    }

    string generatePattern(Dictionary<Vector3Int, int> numbermap)
    {


        // Debug.Log($"{positions[i]} -> {neighbors[i]}");
        // (1,0,0) -> 1

        // 1 for wall -> produce #
        // 2 for no wall -> produce * 
        // important : rest of the time no entry so neighbors is not uniform length!
       
        
        for(int y = 1; y >= -1; --y)
        {
            for(int x = 1; x >= -1; --x)
            {
                if(y == 0 && x == 0)
                {
                    numbermap.Add(new Vector3Int(0, 0, 0), 3);
                }
                Vector3Int cellCoord = new Vector3Int(x, y, 0);
                int dummy = 0;
                if (!numbermap.TryGetValue(cellCoord, out dummy)) numbermap.Add(cellCoord, 0);
            }
        }

        // initialize stringbuilder
        // query the hashmap for the integer and convert that into the character, then append
        // for now lets return the pattern

        // center is 5
        StringBuilder sb = new();
      
        for (int y = 1; y >= -1; --y)
        {
            for(int x = 1; x >= -1; --x)
            {
                Vector3Int key = new Vector3Int(y, x, 0);
                int v = numbermap[key];

                if (v == 0) sb.Append('_');
                if (v == 2) sb.Append('*');
                if (v == 1) sb.Append("#");
                if (v == 3) sb.Append('C');
            }
        }


        return PrettyPattern(sb.ToString());
    }


    string PrettyPattern(string p)
    {
        return
    p.Substring(0, 3) + "\n" +
    p.Substring(3, 3) + "\n" +
    p.Substring(6, 3);
    }



}
