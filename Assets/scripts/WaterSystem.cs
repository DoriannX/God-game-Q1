using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSystem : MonoBehaviour
{
    public static WaterSystem instance { get; private set; }
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private HeightManager heightManager;

    private Queue<Vector3Int> propagationQueue = new();
    private HashSet<Vector3Int> activeTiles = new();
    public HashSet<Vector3Int> waterTiles { get; } = new();
    private readonly List<Vector3Int> pendingRemovals = new();

    private static readonly Vector3Int[] evenNeighborOffsets =
    {
        new(1, 0, 0), new(-1, 0, 0), new(0, 1, 0), new(-1, 1, 0), new(0, -1, 0), new(-1, -1, 0)
    };

    private static readonly Vector3Int[] oddNeighborOffsets =
    {
        new(1, 0, 0), new(-1, 0, 0), new(1, 1, 0), new(0, 1, 0), new(1, -1, 0), new(0, -1, 0)
    };

    private void Awake()
    {
        paintComponent.paintedWater += DrawWater;
        TickSystem.ticked += Tick;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        List<Vector3Int> dryTiles = new List<Vector3Int>();

        foreach (var pos in TilemapManager.instance.waterCellBounds.allPositionsWithin)
        {
            if (TilemapManager.instance.GetWaterTile(pos) == waterTile)
            {
                waterTiles.Add(pos);
            }
            else
            {
                dryTiles.Add(pos);
            }
        }

        foreach (var dry in dryTiles)
        {
            ActivateOneAdjacent(dry);
        }
    }

    private void Tick()
    {
        ProcessPendingRemovals();
        Expand();
    }

    public bool IsOnWater(Vector2 position)
    {
        var cell = TilemapManager.instance.WorldToCell(position);
        return waterTiles.Contains(cell);
    }

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        Vector3Int[] hexNeighbors = (pos.y & 1) == 0 ? evenNeighborOffsets : oddNeighborOffsets;
        int selfHeight = heightManager.GetHeightIndex(TilemapManager.instance.GetTile(pos));
        foreach (Vector3Int n in hexNeighbors)
        {
            int neighborHeight = heightManager.GetHeightIndex(TilemapManager.instance.GetTile(pos + n));
            if (TilemapManager.instance.waterCellBounds.Contains(pos + n) && neighborHeight >= selfHeight)
            {
                yield return pos + n;
            }
        }
    }

    private void Expand()
    {
        int tilesToProcess = propagationQueue.Count;
        for (int i = 0; i < tilesToProcess; i++)
        {
            Vector3Int tile = propagationQueue.Dequeue();
            activeTiles.Remove(tile);

            if (!waterTiles.Contains(tile) && AnyNeighborHasWater(tile))
            {
                DrawWater(tile);
            }
        }
    }

    private bool AnyNeighborHasWater(Vector3Int pos)
    {
        foreach (Vector3Int neighbor in GetNeighbors(pos))
        {
            if (waterTiles.Contains(neighbor))
                return true;
        }

        return false;
    }

    public void AddWater(Vector3Int position)
    {
        waterTiles.Add(position);

        foreach (Vector3Int neighbor in GetNeighbors(position))
        {
            if (activeTiles.Add(neighbor))
                propagationQueue.Enqueue(neighbor);
        }
    }

    public void DrawWater(Vector3Int position)
    {
        AddWater(position);
        TilemapManager.instance.SetWaterTile(position, waterTile);
    }

    public void ActivateOneAdjacent(Vector3Int position)
    {
        foreach (Vector3Int neighbor in GetNeighbors(position))
        {
            if (waterTiles.Contains(neighbor))
            {
                if (activeTiles.Add(position))
                {
                    propagationQueue.Enqueue(position);
                }

                break;
            }
        }
    }

    public void RemoveWater(Vector3Int position)
    {
        if (waterTiles.Remove(position))
        {
            TilemapManager.instance.SetWaterTile(position, null);
            pendingRemovals.Add(position);
        }
    }
    
    private void ProcessPendingRemovals()
    {
        if (pendingRemovals.Count == 0) return;

        int batchSize = 1000;
        int count = Mathf.Min(batchSize, pendingRemovals.Count);

        for (int i = 0; i < count; i++)
        {
            Vector3Int removedTile = pendingRemovals[i];
            ActivateOneAdjacent(removedTile);
        }

        pendingRemovals.RemoveRange(0, count);
    }

    public void ClearAllWater()
    {
        foreach (var pos in waterTiles)
            TilemapManager.instance.SetWaterTile(pos, null);

        waterTiles.Clear();
    }
}