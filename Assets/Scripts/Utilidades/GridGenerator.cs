using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Vector2 gridSize = new Vector2(20, 20);
    public float nodeRadius = 0.5f;
    public LayerMask obstacleMask;
    
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private AStarPathfinding.Nodo[,] grid;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new AStarPathfinding.Nodo[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x/2 - Vector3.forward * gridSize.y/2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + 
                    Vector3.right * (x * nodeDiameter + nodeRadius) + 
                    Vector3.forward * (y * nodeDiameter + nodeRadius);
                
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, obstacleMask);
                grid[x, y] = new AStarPathfinding.Nodo(worldPoint, !walkable);
            }
        }
        LinkNeighbors();
    }

    private void LinkNeighbors()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y].vecinos = new System.Collections.Generic.List<AStarPathfinding.Nodo>();
                // 4 direcciones
                if (x > 0) grid[x, y].vecinos.Add(grid[x-1, y]);
                if (x < gridSizeX-1) grid[x, y].vecinos.Add(grid[x+1, y]);
                if (y > 0) grid[x, y].vecinos.Add(grid[x, y-1]);
                if (y < gridSizeY-1) grid[x, y].vecinos.Add(grid[x, y+1]);
            }
        }
    }

    public AStarPathfinding.Nodo NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - transform.position;
        int x = Mathf.RoundToInt((localPos.x + gridSize.x/2) / nodeDiameter);
        int y = Mathf.RoundToInt((localPos.z + gridSize.y/2) / nodeDiameter);
        return grid[Mathf.Clamp(x, 0, gridSizeX-1), Mathf.Clamp(y, 0, gridSizeY-1)];
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
#endif
        if (grid != null)
        {
            foreach (AStarPathfinding.Nodo n in grid)
            {
#if UNITY_EDITOR
                Gizmos.color = n.esObstaculo ? Color.red : Color.white;
                Gizmos.DrawCube(n.posicion, Vector3.one * (nodeDiameter - 0.1f));
#endif
            }
        }
    }
}
