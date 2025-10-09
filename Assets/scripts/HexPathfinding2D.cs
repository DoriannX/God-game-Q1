using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexPathfinding2D : MonoBehaviour
{
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
        int count = Mathf.Min(batchSize, pendingUpdates.Count);
        for (int i = 0; i < count; i++)
        {
            var cell = pendingUpdates.Dequeue();
            var tile = tilemapManager.GetTile(cell);
            bool isWalkable = tile != null && walkableSet.Contains(tile) && tilemapManager.GetWaterTile(cell) == null;
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

    public Vector3Int? GetRandomWalkableCell(float radius)
    {
        if (walkableCells.Count == 0) return null;
        var list = new List<Vector3Int>(walkableCells);
        int attempts = 100;
        while (attempts-- > 0)
        {
            var cell = list[Random.Range(0, list.Count)];
            if (Vector2.Distance(Vector2.zero, tilemapManager.GetCellCenterWorld(cell)) <= radius)
                return cell;
        }

        return null;
    }

    private void Start()
    {
        heightManager = HeightManager.instance;
        tilemapManager = TilemapManager.instance;
        walkableSet = new HashSet<TileBase>(walkableTiles);
        ComputeWalkableCells();
        TilemapManager.instance.cellChanged += OnTileChanged;
    }

    private void ComputeWalkableCells()
    {
        walkableCells.Clear();
        BoundsInt bounds = tilemapManager.tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                var cell = new Vector3Int(x, y, 0);
                TileBase tile = tilemapManager.GetTile(cell);
                if (tile != null && walkableSet.Contains(tile) && tilemapManager.GetWaterTile(cell) == null)
                    walkableCells.Add(cell);
            }
        }
    }

    public List<Vector2> GetWorldPath(List<Vector3Int> path)
    {
        if (path == null) return null;
        int count = path.Count;
        var worldPath = new List<Vector2>(count);
        for (int i = 0; i < count; i++)
            worldPath.Add(tilemapManager.GetCellCenterWorld(path[i]));
        return worldPath;
    }

    public List<Vector3Int> FindPath(Vector2 startWorld, Vector2 goalWorld)
    {
        return FindPath(tilemapManager.WorldToCell(startWorld), tilemapManager.WorldToCell(goalWorld));
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        // Vérifie que start et goal sont dans les cases marchables
        if (!walkableCells.Contains(start) || !walkableCells.Contains(goal))
            return null;

        var openSet = new PriorityQueue<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;

        var neighborBuffer = new Vector3Int[6];

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == goal)
                return Reconstruct(cameFrom, current);

            int nCount = GetNeighbors(current, neighborBuffer);
            float currentG = gScore[current];

            for (int i = 0; i < nCount; i++)
            {
                var neighbor = neighborBuffer[i];

                if (!walkableCells.Contains(neighbor))
                    continue;
                if (!IsHeightWalkable(tilemapManager.GetTile(current), tilemapManager.GetTile(neighbor)))
                    continue;

                float tentative = currentG + 1f;
                if (gScore.TryGetValue(neighbor, out float existingG) && tentative >= existingG)
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative;
                float f = tentative + Heuristic(neighbor, goal);
                openSet.Enqueue(neighbor, f);
            }
        }

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