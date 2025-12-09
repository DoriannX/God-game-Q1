using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class HexPathfinding3D : MonoBehaviour
{
    public static HexPathfinding3D instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }
    private HashSet<GameObject> walkableSet;
    private HashSet<Vector3Int> walkableCells = new();
    private TilemapManager tilemapManager;
    [SerializeField] private GameObject[] walkableTiles;
    private readonly Queue<Vector3Int> pendingUpdates = new();

    public void OnTileChanged(Vector3Int cell)
    {
        pendingUpdates.Enqueue(cell);
    }

    private void Update()
    {
        int batchSize = 50;    
        int maxProcessed = Mathf.Min(1000, pendingUpdates.Count);
        int count = Mathf.Min(batchSize, maxProcessed);

        for (int i = 0; i < count; i++)
        {
            if (pendingUpdates.Count == 0) break;
            var cell = pendingUpdates.Dequeue();
            var tile = tilemapManager.GetTile(cell);
            
            bool isWalkable = tile != null && walkableSet.Contains(tile);
            
            if (isWalkable) walkableCells.Add(cell);
            else walkableCells.Remove(cell);
        }
    }

    static readonly Vector2Int[] evenRowNeighbors =
    {
        new(+1, 0), new(0, +1), new(-1, +1),
        new(-1, 0), new(-1, -1), new(0, -1),
    };

    static readonly Vector2Int[] oddRowNeighbors =
    {
        new(+1, 0), new(+1, +1), new(0, +1),
        new(-1, 0), new(0, -1), new(+1, -1),
    };

    private void Start()
    {
        tilemapManager = TilemapManager.instance;
        walkableSet = new HashSet<GameObject>(walkableTiles);
        ComputeWalkableCells();
        TilemapManager.instance.cellChanged += OnTileChanged;
    }

    private void OnDestroy()
    {
        if (TilemapManager.instance != null)
        {
            TilemapManager.instance.cellChanged -= OnTileChanged;
        }
    }

    public void ComputeWalkableCells()
    {
        walkableCells.Clear();
        BoundsInt bounds = tilemapManager.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cell = new Vector3Int(x, y, 0);
                GameObject tile = tilemapManager.GetTile(cell);
                
                if (tile != null && walkableSet.Contains(tile))
                    walkableCells.Add(cell);
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 goalWorld, int startHeight = 0, int goalHeight = 0) {
        
        // Convert world positions to grid coordinates
        Vector3Int startCell = tilemapManager.WorldToHexAxial(startWorld);
        startCell.z = startHeight;
        
        Vector3Int goalCell = tilemapManager.WorldToHexAxial(goalWorld);
        goalCell.z = goalHeight;
    
        // Check if start and goal are walkable
        if (!walkableCells.Contains(new Vector3Int(startCell.x, startCell.y, 0)) || 
            !walkableCells.Contains(new Vector3Int(goalCell.x, goalCell.y, 0)))
            return null;
    
        // Initialize A* data structures
        var openSet = new PriorityQueue<Vector3Int>();
        var parentCell = new Dictionary<Vector3Int, Vector3Int>();
        var costFromStart = new Dictionary<Vector3Int, float>();
    
        openSet.Enqueue(startCell, 0);
        costFromStart[startCell] = 0;
    
        var neighborBuffer = new Vector3Int[8];
    
        // Main A* loop
        while (openSet.Count > 0)
        {
            Vector3Int currentCell = openSet.Dequeue();
    
            // Goal reached - reconstruct path
            if (currentCell == goalCell)
            {
                var path = new List<Vector3>();
                while (parentCell.TryGetValue(currentCell, out Vector3Int previousCell))
                {
                    path.Add(tilemapManager.HexAxialToWorld(currentCell));
                    currentCell = previousCell;
                }
                path.Add(tilemapManager.HexAxialToWorld(currentCell));
                path.Reverse();
                openSet.Dispose();
                return path;
            }
    
            // Explore neighbors (6 horizontal + vertical for 3D pathfinding)
            int neighborCount = GetNeighbors3D(currentCell, neighborBuffer);
            float currentCost = costFromStart[currentCell];
    
            for (int i = 0; i < neighborCount; i++)
            {
                var neighborCell = neighborBuffer[i];
    
                // Skip if not walkable or height difference too large
                if (!walkableCells.Contains(new Vector3Int(neighborCell.x, neighborCell.y, 0)))
                    continue;
                if (!IsHeightWalkable(tilemapManager.GetTile(new Vector3Int(currentCell.x, currentCell.y, 0)), 
                                     tilemapManager.GetTile(new Vector3Int(neighborCell.x, neighborCell.y, 0))))
                    continue;
    
                // Calculate new cost (g score) - account for vertical movement
                float newCost = currentCost + GetMovementCost(currentCell, neighborCell);
    
                // Skip if not better than existing path
                if (costFromStart.TryGetValue(neighborCell, out float existingCost) && newCost >= existingCost)
                    continue;
    
                // Update best path to this neighbor
                parentCell[neighborCell] = currentCell;
                costFromStart[neighborCell] = newCost;
                
                // Calculate f = g + h and add to priority queue
                float priorityScore = newCost + Heuristic3D(neighborCell, goalCell);
                openSet.Enqueue(neighborCell, priorityScore);
            }
        }
    
        // No path found
        openSet.Dispose();
        return null;
    }

    int GetNeighbors(Vector3Int pos, Vector3Int[] buffer)
    {
        var offsets = (pos.y & 1) == 0 ? evenRowNeighbors : oddRowNeighbors;
        for (int i = 0; i < 6; i++)
            buffer[i] = new Vector3Int(pos.x + offsets[i].x, pos.y + offsets[i].y, 0);
        return 6;
    }

    int GetNeighbors3D(Vector3Int pos, Vector3Int[] buffer)
    {
        var offsets = (pos.y & 1) == 0 ? evenRowNeighbors : oddRowNeighbors;
        int count = 0;
        
        // Add 6 horizontal neighbors at same height
        for (int i = 0; i < 6; i++)
        {
            buffer[count++] = new Vector3Int(pos.x + offsets[i].x, pos.y + offsets[i].y, pos.z);
        }
        
        // Add vertical neighbors (up and down in height)
        buffer[count++] = new Vector3Int(pos.x, pos.y, pos.z + 1);
        buffer[count++] = new Vector3Int(pos.x, pos.y, pos.z - 1);
        
        return count;
    }

    float GetMovementCost(Vector3Int from, Vector3Int to)
    {
        // Horizontal movement costs 1
        if (from.z == to.z)
            return 1f;
        
        // Vertical movement costs more (climbing/descending)
        int heightDiff = Mathf.Abs(to.z - from.z);
        return 1f + (heightDiff * 0.5f);
    }

    bool IsHeightWalkable(GameObject fromTile, GameObject toTile)
    {
        if (fromTile == null || toTile == null)
            return false;
        
        return true;
            
        /*int fromH = heightManager.GetHeightIndex(fromTile);
        int toH = heightManager.GetHeightIndex(toTile);
        return Mathf.Abs(fromH - toH) <= 1;*/
    }

    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    float Heuristic3D(Vector3Int a, Vector3Int b)
    {
        // Manhattan distance in XY + height difference
        float horizontalDist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        float verticalDist = Mathf.Abs(a.z - b.z) * 0.5f;
        return horizontalDist + verticalDist;
    }

    List<Vector3Int> Reconstruct(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3Int>();
        while (cameFrom.TryGetValue(current, out var prev))
        {
            path.Add(current);
            current = prev;
        }

        path.Add(current);
        path.Reverse();
        return path;
    }
}
