using UnityEngine;

public static class GameUtils 
{
    public static Vector3 GetWorldPosition(int width, int height , int x, int z)
    {
        float offsetX = (width - 1) / 2f;
        float offsetZ = (height - 1) / 2f;
        return new Vector3(x - offsetX, 0, z - offsetZ);
    }

    public static Vector3 GetWorldPosition(int width, int height,float spacing, int x, int z)
    {
        float offsetX = (width - 1) * spacing / 2f;
        float offsetZ = (height - 1) * spacing / 2f;
        return new Vector3(x * spacing - offsetX, 0, z * spacing - offsetZ);
    }

    public static Vector3 GetWorldPositionFromCoords(Vector2Int coords, GridDataSO data)
    {
        return GetWorldPosition(data.GridWidth, data.GridHeight, data.CellSpacing, coords.x, coords.y);
    }

    public static string Colorize(string text, string color)
    {
        return $"<color={color}>{text}</color>";
    }
}
