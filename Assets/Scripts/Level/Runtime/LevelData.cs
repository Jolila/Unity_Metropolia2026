using UnityEngine;

public class LevelData
{
    public static LevelData Instance;


    public int PlayerX { get; set; }
    public int PlayerY { get; set; }

    public char[,] Geometry { get; set; }
    public char[,] Decorations { get; set; }
    public char[,] Outline { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public int OutlinePadding = 10;

    public int OutlineWidth { get; set; }
    public int OutlineHeight { get; set; }


    public void Initialize(char[,] grid, int width, int height)
    {
        Geometry = grid;
        Width = width;
        Height = height;
    }

    public bool IsWall(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.y);

        if (x < 0 || y < 0 || x >= Width || y >= Height)
            return true;

        return Geometry[x, y] == '#';
    }
}
