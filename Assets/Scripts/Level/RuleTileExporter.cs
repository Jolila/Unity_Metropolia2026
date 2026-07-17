using System.Collections.Generic;
using System.Reflection;
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

            for (int i = 0; i < neighbors.Count; i++)
            {
                Debug.Log($"{positions[i]} -> {neighbors[i]}");
            }

        }


        
        
    }




    string InspectObject(object o)
    {
        StringBuilder sb = new();
        foreach (var field in o.GetType().GetFields(
            BindingFlags.Instance |
               BindingFlags.Public |
               BindingFlags.NonPublic))
        {

            object value = field.GetValue(o);
            sb.Append($"{field.Name} = {field.GetValue(o)}");
        }

        return sb.ToString();
    }
}
