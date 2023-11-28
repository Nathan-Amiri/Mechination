using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridIndex
{
    public static Dictionary<Vector2, Node> gridIndex = new();

    public static void UpdateIndexPosition(Node node, Vector2 oldPosition, Vector2 newPosition)
    {
        gridIndex.Remove(oldPosition);
        gridIndex.Add(newPosition, node);
    }
}