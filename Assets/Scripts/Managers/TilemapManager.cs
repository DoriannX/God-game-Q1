using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SaveableEntity))]
public class TilemapManager : MonoBehaviour, ISaveable
{
    [Serializable]
    private struct TilemapContainer
    {
        public List<SaveableEntity.Vector3Data> positions;
        public List<string> tilespath;
    }

    [Serializable]
    private struct TilemapData
    {
        public TilemapContainer tilemap;
        public TilemapContainer waterTilemap;
    }

    [field: SerializeField] public int limitX { get; private set; } = 90;
    [field: SerializeField] public int limitY { get; private set; } = 100;
    public Action<Vector3Int> cellChanged;

    public Action<Vector3Int> onWaterCellChanged;
    public BoundsInt cellBounds => new BoundsInt(new Vector3Int(-90, -100, 0), new Vector3Int(180, 200, 1));
    public BoundsInt waterCellBounds => new BoundsInt(new Vector3Int(-90, -100, 0), new Vector3Int(180, 200, 1));
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

    [SerializeField] private Tilemap tilemap;
    [ SerializeField] private Tilemap waterTilemap;

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
        if (Mathf.Abs(cell.x) > limitX || Mathf.Abs(cell.y) > limitY)
            return;
        tilemap.SetTile(cell, tile);
        cellChanged?.Invoke(cell); 
        UpdateTileCache(cell);
    }

    public void SetWaterTile(Vector3Int cell, TileBase tile)
    {
        if (Mathf.Abs(cell.x) > limitX || Mathf.Abs(cell.y) > limitY)
            return;
        waterTilemap.SetTile(cell, tile);
        onWaterCellChanged?.Invoke(cell);
    }

    public Vector3Int WorldToWaterCell(Vector2 pos)
    {
        return waterTilemap.WorldToCell(pos);
    }

    public Vector3 GetWaterCellCenterWorld(Vector3Int cell)
    {
        return waterTilemap.GetCellCenterWorld(cell);
    }

    public bool NeedsToBeSaved()
    {
        return true;
    }

    public bool NeedsReinstantiation()
    {
        return false;
    }

    public object SaveState()
    {
        TilemapData data = new TilemapData();
        data.tilemap.positions = new List<SaveableEntity.Vector3Data>();
        data.tilemap.tilespath = new List<string>();
        for (int x = -limitX; x < limitX; x++)
        {
            for (int y = -limitY; y < limitY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                {
                    data.tilemap.positions.Add(new SaveableEntity.Vector3Data(pos));
                    data.tilemap.tilespath.Add("Tiles/" + tile.name);
                }
            }
        }

        data.waterTilemap = new TilemapContainer();
        data.waterTilemap.positions = new List<SaveableEntity.Vector3Data>();
        data.waterTilemap.tilespath = new List<string>();
        for (int x = -limitX; x < limitX; x++)
        {
            for (int y = -limitY; y < limitY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = waterTilemap.GetTile(pos);
                if (tile != null)
                {
                    data.waterTilemap.positions.Add(new SaveableEntity.Vector3Data(pos));
                    data.waterTilemap.tilespath.Add("Tiles/" + tile.name);
                }
            }
        }

        return data;
    }

    public void LoadState(object state)
    {
        var data = (TilemapData)state;
        for (int i = 0; i < limitX; i++)
        {
            for (int j = 0; j < limitY; j++)
            {
                Vector3Int pos = new Vector3Int(tilemap.cellBounds.xMin + i, tilemap.cellBounds.yMin + j, 0);
                SetTile(pos, null);
            }
        }
        WaterSystem.instance.ClearAllWater();
        for (int i = 0; i < data.tilemap.positions.Count; i++)
        {
            Vector3Int pos = new Vector3Int((int)data.tilemap.positions[i].x, (int)data.tilemap.positions[i].y,
                (int)data.tilemap.positions[i].z);
            SetTile(pos, Resources.Load<TileBase>(data.tilemap.tilespath[i]));
        }

        for (int i = 0; i < data.waterTilemap.positions.Count; i++)
        {
            Vector3Int pos = new Vector3Int((int)data.waterTilemap.positions[i].x,
                (int)data.waterTilemap.positions[i].y, (int)data.waterTilemap.positions[i].z);
            WaterSystem.instance.DrawWater(pos);
        }

        tileCache.Clear();
        HexPathfinding2D.instance.ComputeWalkableCells();
    }

    public void PostInstantiation(object state)
    {
        // Nothing to do here
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }

    public Vector2 CellToWorld(Vector3Int vector3Int)
    {
        return tilemap.CellToWorld(vector3Int);
    }
}