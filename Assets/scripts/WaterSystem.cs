using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterSystem : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private TileBase waterTile;
    private readonly List<Vector3Int> waterTiles = new();
    private List<Vector3Int> completedWaterTiles = new();
    [SerializeField] private HeightManager heightManager;

    private void Awake()
    {
        paintComponent.paintedWater += AddWaterTile;
        TickSystem.ticked += Tick;
    }

    private void Start()
    {
        if (waterTilemap == null || waterTilemap.cellBounds.size == Vector3Int.zero)
        {
            return;
        }

        foreach (var pos in waterTilemap.cellBounds.allPositionsWithin)
        {
            if (waterTilemap.GetTile(pos) == waterTile)
            {
                waterTiles.Add(pos);
            }
        }
    }

    private void MarkTileCompleted(Vector3Int position)
    {
        if (!completedWaterTiles.Contains(position))
            completedWaterTiles.Add(position);

        waterTiles.Remove(position);

        // Ensure tile exists and color it red
        if (waterTilemap.GetTile(position) != waterTile)
            waterTilemap.SetTile(position, waterTile);
    }

    private void Tick()
    {
        CheckAllWaterTiles();
    }

    // Neighbor offsets (pointy-top hex with row parity)
    private static readonly Vector3Int[] evenNeighborOffsets =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, -1, 0)
    };

    private static readonly Vector3Int[] oddNeighborOffsets =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 0)
    };

    private void CheckAllWaterTiles()
    {
        if (waterTiles.Count == 0) return;

        // First pass: expand
        var snapshot = waterTiles.ToList();
        foreach (var pos in snapshot)
            ExpandFrom(pos);

        // Second pass: completion check
        snapshot = waterTiles.ToList();
        foreach (var pos in snapshot)
        {
            if (IsTileCompleted(pos))
            {
                MarkTileCompleted(pos);
            }
        }
    }

    private void ExpandFrom(Vector3Int position)
    {
        int currentHeight = heightManager.GetHeightIndex(tilemap.GetTile(position));
        var offsets = ((position.y & 1) == 0) ? evenNeighborOffsets : oddNeighborOffsets;

        foreach (var off in offsets)
        {
            var neighbor = position + off;

            if (waterTiles.Contains(neighbor) || completedWaterTiles.Contains(neighbor))
                continue;

            var neighborTile = tilemap.GetTile(neighbor);
            if (heightManager.GetHeightIndex(neighborTile) > currentHeight)
                continue;

            AddWaterTile(neighbor);
            waterTilemap.SetTile(neighbor, waterTile);
        }
    }

    private bool IsTileCompleted(Vector3Int position)
    {
        var offsets = ((position.y & 1) == 0) ? evenNeighborOffsets : oddNeighborOffsets;
        foreach (var off in offsets)
        {
            var neighbor = position + off;
            if (!waterTiles.Contains(neighbor) && !completedWaterTiles.Contains(neighbor))
                return false;
        }

        return true;
    }

    private void AddWaterTile(Vector3Int position)
    {
        if (waterTiles.Contains(position) || completedWaterTiles.Contains(position))
        {
            return;
        }

        waterTiles.Add(position);
    }
    
    public void RemoveWaterTile(Vector3Int position)
    {
        if (waterTiles.Remove(position) || completedWaterTiles.Remove(position))
        {
            waterTilemap.SetTile(position, null);
        }
    }

    public void ReactivateAdjacentWater(Vector3Int position)
    {
        void Reactivate(Vector3Int p)
        {
            if (completedWaterTiles.Remove(p))
            {
                if (!waterTiles.Contains(p))
                    waterTiles.Add(p);
            }
            else if (waterTilemap.GetTile(p) == waterTile && !waterTiles.Contains(p))
            {
                waterTiles.Add(p);
            }
        }

        Reactivate(position);

        var offsets = (position.y & 1) == 0 ? evenNeighborOffsets : oddNeighborOffsets;
        foreach (var off in offsets)
            Reactivate(position + off);
    }
}