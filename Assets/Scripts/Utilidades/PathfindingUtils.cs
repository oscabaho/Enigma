// PathfindingUtils.cs
using UnityEngine;
using System.Collections.Generic;

public static class PathfindingUtils
{
    // Ejemplo: Calcula la heurística Manhattan para grids
    public static float HeuristicaManhattan(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
