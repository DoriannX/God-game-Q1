using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// TEMPORARILY MODIFIED - Water painting disabled for new hexagonal TilemapManager.
/// Core structure preserved for future re-implementation.
/// </summary>
public class PaintComponent : MonoBehaviour
{
    [Header("WATER SYSTEM DISABLED")]
    [SerializeField] private TileBase waterTile;
    
    // Event kept for compatibility but not invoked
    public event Action<Vector3Int> paintedWater;

    public void Add(Vector2 mousePos, float brushSize)
    {
        // DISABLED: Water painting temporarily disabled
        Add(mousePos, brushSize, TileSelector.instance.GetCurrentTilePrefab());
    }
    
    public void Up(Vector2 mousePos, float brushSize)
    {
        TilemapManager.instance.RaiseTile(mousePos);
    }

    public void Add(Vector2 mousePos, float brushSize, GameObject tile)
    {
        // DISABLED: Water painting temporarily disabled
        TilemapManager.instance.PlaceTile(mousePos, tile);
    }
    public void SetCurrentTile(int index)
    {
        TileSelector.instance.SetCurrentTileIndex(index);
    }
}