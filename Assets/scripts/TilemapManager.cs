using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public Action<Vector3Int> cellChanged;
    public BoundsInt cellBounds => tilemap.cellBounds;
    public BoundsInt waterCellBounds => waterTilemap.cellBounds;
    public static TilemapManager instance { get; private set; }
    
    private readonly Dictionary<Vector3Int, TileBase> tileCache = new();
    
    public TileBase GetTile(Vector3Int cell)
    {
        if (tileCache.TryGetValue(cell, out TileBase cached))
            return cached;

        var t = tilemap.GetTile(cell);
        tileCache[cell] = t;
        return t;
    }

    private void UpdateTileCache(Vector3Int cell)
    {
        tileCache[cell] = tilemap.GetTile(cell);
    }
    
    public Vector3 cellSize => tilemap.cellSize;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    [field:SerializeField] public Tilemap tilemap { get; private set; }
    [field: SerializeField] public Tilemap waterTilemap { get; private set; }
    
    public Vector3 GetCellCenterWorld(Vector3Int cellPosition) => tilemap.GetCellCenterWorld(cellPosition);

    public Vector3Int WorldToCell(Vector2 startWorld)
    {
        return tilemap.WorldToCell(startWorld);
    }

    public TileBase GetWaterTile(Vector3Int cell)
    {
        return waterTilemap.GetTile(cell);
    }

    public void SetTile(Vector3Int cell, TileBase tile)
    {
        tilemap.SetTile(cell, tile);
        cellChanged?.Invoke(cell);
        UpdateTileCache(cell);
    }

    public void SetWaterTile(Vector3Int cell, TileBase tile)
    {
        waterTilemap.SetTile(cell, tile);
        cellChanged?.Invoke(cell);
    }

    public Vector3Int WorldToWaterCell(Vector2 pos)
    {
        return waterTilemap.WorldToCell(pos);
    }

    public Vector3 GetWaterCellCenterWorld(Vector3Int cell)
    {
        return waterTilemap.GetCellCenterWorld(cell);
    }
}
