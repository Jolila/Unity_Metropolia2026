using System.IO;
using UnityEngine;
using UnityEditor;

public class LevelCommitter : MonoBehaviour
{


    [ContextMenu("Commit level")]
    private void Commit()
    {
        string workingFolder = "Assets/Generated/Working/";
        string readyFolder = "Assets/Generated/Ready";

        string[] geometryFiles = Directory.GetFiles(workingFolder, "*_geometry.txt");
        string geometryPath = geometryFiles[0];

        string geometryName = Path.GetFileNameWithoutExtension(geometryPath);
        string baseName = geometryName.Split('_')[0];

        string levelFolder = Path.Combine("Assets/Generated/Ready", baseName);
        Directory.CreateDirectory(levelFolder);

        MoveFile(Path.Combine(workingFolder, $"{baseName}_geometry.txt"),
             Path.Combine(levelFolder, "geometry.txt"));

        MoveFile(Path.Combine(workingFolder, $"{baseName}_decorations.txt"),
                     Path.Combine(levelFolder, "decorations.txt"));

        MoveFile(Path.Combine(workingFolder, $"{baseName}_info.txt"),
                     Path.Combine(levelFolder, "info.txt"));

        MoveFile(Path.Combine(workingFolder, $"{baseName}_config.txt"),
                     Path.Combine(levelFolder, "config.txt"));

        //AssetDatabase.Refresh();
    }


    private static void MoveFile(string source, string destination)
    {
        

        if (File.Exists(destination))
            File.Delete(destination);

        File.Move(source, destination);
    }
}
