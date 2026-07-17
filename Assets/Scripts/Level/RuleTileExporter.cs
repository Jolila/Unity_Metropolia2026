using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;



public class RuleTileExporter : MonoBehaviour
{

    [SerializeField] RuleTile Rules;


    [ContextMenu("Export Rule Tileset")]
    public void ExportRuleset()
    {
        RuleTile tile = Selection.activeObject as RuleTile;

        StringBuilder sb = new();

  
        FieldInfo field = typeof(RuleTile).GetField(
            "m_TilingRules",
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Public);

        var list = (List<RuleTile.TilingRule>)field.GetValue(Rules);

        Debug.Log($"Rules: {list.Count}");

        foreach (var ruleOf in list)
            {
                Debug.Log(InspectObject(ruleOf));
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
