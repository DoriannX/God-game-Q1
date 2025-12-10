using System;
using System.Collections.Generic;
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

    public void Add()
    {
        TilemapManager.instance.PlaceTilesAtMouse(TileSelector.instance.GetCurrentTile());
    }
    public void SetCurrentTile(int index)
    {
        TileSelector.instance.SetCurrentTileIndex(index);
    }
}