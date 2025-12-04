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
    [field: SerializeField] public Tile[] tiles { get; private set; }
    
    public int tileIndex { get; private set; }
    private Vector2 lastPos;
    private Vector2 lastHitPos;
    
    // Event kept for compatibility but not invoked
    public event Action<Vector3Int> paintedWater;

    public void Add(Vector2 pos, float brushSize)
    {
        // DISABLED: Water painting temporarily disabled
        Debug.LogWarning("PaintComponent.Add() is DISABLED - Use TilemapManagerCopy for tile placement");
    }

    public void Add(Vector2 pos, float brushSize, TileBase tile)
    {
        // DISABLED: Water painting temporarily disabled
        Debug.LogWarning("PaintComponent.Add() is DISABLED - Use TilemapManagerCopy for tile placement");
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