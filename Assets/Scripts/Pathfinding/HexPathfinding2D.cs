using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class HexPathfinding2D : MonoBehaviour
{
    public static HexPathfinding2D instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }
    private HeightManager heightManager;
    private HashSet<TileBase> walkableSet;
    private HashSet<Vector3Int> walkableCells = new();
    private TilemapManager tilemapManager;
    [SerializeField] private TileBase[] walkableTiles;
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
            
            // DISABLED: Water tile checking removed (water system disabled)
            // Walkability now only checks if tile exists and is in walkable set
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
        heightManager = HeightManager.instance;
        tilemapManager = TilemapManager.instance;
        walkableSet = new HashSet<TileBase>(walkableTiles);
        ComputeWalkableCells();
        TilemapManager.instance.cellChanged += OnTileChanged;
        
        // DISABLED: Water cell changed event removed (water system disabled)
        // TilemapManager.instance.onWaterCellChanged += OnTileChanged;
    }

    private void OnDestroy()
    {
        if (TilemapManager.instance != null)
        {
            TilemapManager.instance.cellChanged -= OnTileChanged;
            
            // DISABLED: Water cell changed event removed (water system disabled)
            // TilemapManager.instance.onWaterCellChanged -= OnTileChanged;
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
                TileBase tile = tilemapManager.GetTile(cell);
                
                // DISABLED: Water tile checking removed (water system disabled)
                // Walkability now only checks if tile exists and is in walkable set
                if (tile != null && walkableSet.Contains(tile))
                    walkableCells.Add(cell);
            }
        }
    }

    public List<Vector2> FindPath(Vector2 startWorld, Vector2 goalWorld)
    {
        // Convert world positions to grid coordinates
        Vector3Int startCell = tilemapManager.WorldToCell(startWorld);
        Vector3Int goalCell = tilemapManager.WorldToCell(goalWorld);
    
        // Check if start and goal are walkable
        if (!walkableCells.Contains(startCell) || !walkableCells.Contains(goalCell))
            return null;
    
        // Initialize A* data structures
        //f = g + h
        //h = heuristic cost to goal
        var openSet = new PriorityQueue<Vector3Int>();  // Cells to explore, sorted by f score
        var parentCell = new Dictionary<Vector3Int, Vector3Int>();  // Track path for reconstruction
        var costFromStart = new Dictionary<Vector3Int, float>();  // g score: actual cost from start
    
        openSet.Enqueue(startCell, 0);
        costFromStart[startCell] = 0;
    
        var neighborBuffer = new Vector3Int[6];  // Reusable buffer to avoid allocations
    
        // Main A* loop
        while (openSet.Count > 0)
        {
            Vector3Int currentCell = openSet.Dequeue();
    
            // Goal reached - reconstruct path
            if (currentCell == goalCell)
            {
                var path = new List<Vector2>();
                while (parentCell.TryGetValue(currentCell, out Vector3Int previousCell))
                {
                    path.Add(tilemapManager.GetCellCenterWorld(currentCell));
                    currentCell = previousCell;
                }
                path.Add(tilemapManager.GetCellCenterWorld(currentCell));
                path.Reverse();
                openSet.Dispose();
                return path;
            }
    
            // Explore neighbors
            int neighborCount = GetNeighbors(currentCell, neighborBuffer);
            float currentCost = costFromStart[currentCell];
    
            for (int i = 0; i < neighborCount; i++)
            {
                var neighborCell = neighborBuffer[i];
    
                // Skip if not walkable or height difference too large
                if (!walkableCells.Contains(neighborCell))
                    continue;
                if (!IsHeightWalkable(tilemapManager.GetTile(currentCell), tilemapManager.GetTile(neighborCell)))
                    continue;
    
                // Calculate new cost (g score)
                float newCost = currentCost + 1f;
    
                // Skip if not better than existing path
                if (costFromStart.TryGetValue(neighborCell, out float existingCost) && newCost >= existingCost)
                    continue;
    
                // Update best path to this neighbor
                parentCell[neighborCell] = currentCell;
                costFromStart[neighborCell] = newCost;
                
                // Calculate f = g + h and add to priority queue
                float priorityScore = newCost + Heuristic(neighborCell, goalCell);
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

    bool IsHeightWalkable(TileBase fromTile, TileBase toTile)
    {
        int fromH = heightManager.GetHeightIndex(fromTile);
        int toH = heightManager.GetHeightIndex(toTile);
        return Mathf.Abs(fromH - toH) <= 1;
    }

    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
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