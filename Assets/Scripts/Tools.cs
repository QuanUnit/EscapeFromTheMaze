using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static List<T> GetNeighbours<T>(this T[,] array, int i, int j)
    {
        List<T> output = new List<T>();

        Vector2Int[] directions = { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, -1) };

        foreach (var direction in directions)
        {
            Vector2Int neighboursIndx = new Vector2Int(direction.x + i, direction.y + j);

            if (neighboursIndx.x >= 0 && j + neighboursIndx.y >= 0 &&
                neighboursIndx.x < array.GetLength(0) && neighboursIndx.y < array.GetLength(1)
                && array[neighboursIndx.x, neighboursIndx.y] != null)
                output.Add(array[neighboursIndx.x, neighboursIndx.y]);
        }
        return output;
    }
}
