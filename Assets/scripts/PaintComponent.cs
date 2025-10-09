using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PaintComponent : MonoBehaviour
{
    [SerializeField] private TileBase waterTile;
    [field: SerializeField] public Tile[] tiles { get; private set; }
    //Create objects and put them here
    public int tileIndex { get; private set; }
    private Vector2 lastPos;
    private Vector2 lastHitPos;
    public event Action<Vector3Int> paintedWater;

    public void Add(Vector2 pos, float brushSize)
    {
        Add(pos, brushSize, tiles[tileIndex]);
    }

    public void Add(Vector2 pos, float brushSize, TileBase tile)
    {
        Vector3Int centerCell = TilemapManager.instance.WorldToCell(pos);
        
        int cellRadiusX = Mathf.CeilToInt(brushSize * 1.5f / TilemapManager.instance.cellSize.x);
        int cellRadiusY = Mathf.CeilToInt(brushSize * 1.5f / TilemapManager.instance.cellSize.y);
        
        int maxRadius = Mathf.Max(cellRadiusX, cellRadiusY);
        
        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = TilemapManager.instance.GetCellCenterWorld(cell);
        
                if (Vector2.Distance(pos, cellWorldPos) <= brushSize)
                {
                    if (tile == waterTile)
                    {
                        paintedWater?.Invoke(cell);
                        TilemapManager.instance.SetWaterTile(cell, tile);
                    }
                    else
                    {
                        TilemapManager.instance.SetTile(cell, tile);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastPos, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lastHitPos, 0.5f);
    }

    public void SetCurrentTile(int index)
    {
        tileIndex = index;
    }
}