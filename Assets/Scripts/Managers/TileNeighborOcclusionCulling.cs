using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Neighbor-based occlusion culling system
/// Disables tiles completely surrounded by other tiles (no raycast, ultra-performant)
/// </summary>
public class TileNeighborOcclusionCulling : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TilemapManager tilemapManager;
    
    [Header("Settings")]
    [SerializeField] private bool enableOcclusion = true;
    [SerializeField] private bool occludeBaseTiles = true;
    [SerializeField] private bool occludeHeightTiles = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo;
    
    // Cache of occlusion states by coordinate
    private Dictionary<Vector3Int, bool> occlusionCache = new();
    
    // Batch processing
    private bool isBatchMode;
    private HashSet<Vector2Int> batchedColumns = new();
    
    // Stats
    private int totalOccludedTiles;
    private int totalVisibleTiles;
    
    // The 6 hexagonal directions (flat-top)
    private readonly Vector2Int[] hexDirections = new Vector2Int[]
    {
        new(1, 0),   // East
        new(-1, 0),  // West
        new(0, 1),   // North-East
        new(0, -1),  // South-West
        new(1, -1),  // South-East
        new(-1, 1)   // North-West
    };

    private void OnEnable()
    {
        tilemapManager.columnModified += UpdateOcclusionForColumn;
        tilemapManager.startedPlacingTiles += BeginBatch;
        tilemapManager.endedPlacingTiles += EndBatch;
    }

    private void OnDisable()
    {
        tilemapManager.columnModified -= UpdateOcclusionForColumn;
        tilemapManager.startedPlacingTiles -= BeginBatch;
        tilemapManager.endedPlacingTiles -= EndBatch;
    }

    /// <summary>
    /// Checks and updates occlusion for a column and its neighbors
    /// </summary>
    public void UpdateOcclusionForColumn(Vector3Int hexCoords)
    {
        if (!enableOcclusion || tilemapManager == null)
            return;
        
        Vector2Int coords2D = new Vector2Int(hexCoords.x, hexCoords.y);
        
        // In batch mode, accumulate columns to process
        if (isBatchMode)
        {
            batchedColumns.Add(coords2D);
            foreach (Vector2Int direction in hexDirections)
            {
                batchedColumns.Add(coords2D + direction);
            }
            return;
        }
        
        // Update the column itself
        UpdateColumnOcclusion(coords2D);
        
        // Update the 6 neighbors (as this column may now occlude them)
        foreach (Vector2Int direction in hexDirections)
        {
            Vector2Int neighborCoords = coords2D + direction;
            UpdateColumnOcclusion(neighborCoords);
        }
    }
    
    /// <summary>
    /// Starts batch mode to optimize massive updates
    /// </summary>
    public void BeginBatch()
    {
        isBatchMode = true;
        batchedColumns.Clear();
    }
    
    /// <summary>
    /// Ends batch mode and processes all accumulated columns
    /// </summary>
    public void EndBatch()
    {
        isBatchMode = false;
        
        // Process all accumulated columns
        foreach (Vector2Int coords in batchedColumns)
        {
            UpdateColumnOcclusion(coords);
        }
        
        batchedColumns.Clear();
    }
    
    /// <summary>
    /// Updates occlusion for a complete column
    /// </summary>
    private void UpdateColumnOcclusion(Vector2Int hexCoords)
    {
        if (tilemapManager == null)
            return;
        
        // Get column height by finding the max z coordinate for this q,r position
        int topCoordinate = TilemapManager.instance.GetColumnTopCoordinate(hexCoords);
        
        if (topCoordinate == 0)
            return; // No tiles in this column
        
        // Check each tile in the column
        for (int tileHeight = 0; tileHeight < topCoordinate; tileHeight++)
        {
            bool shouldOcclude = ShouldOccludeTile(hexCoords, tileHeight, topCoordinate);
            ApplyOcclusion(hexCoords, tileHeight, shouldOcclude);
        }
    }
    
    /// <summary>
    /// Determines if a tile should be occluded
    /// </summary>
    private bool ShouldOccludeTile(Vector2Int hexCoords, int tileHeight, int topCoordinate)
    {
        // Don't occlude the top tile (always visible)
        if (tileHeight == topCoordinate)
            return false;
        
        // Check tile type
        if (tileHeight == 1 && !occludeBaseTiles)
            return false;
        
        if (tileHeight > 1 && !occludeHeightTiles)
            return false;
        
        bool allNeighborsHigherOrEqual = true;
        
        foreach (Vector2Int direction in hexDirections)
        {
            Vector2Int neighborCoords = hexCoords + direction;
            int neighborHeight = TilemapManager.instance.GetColumnTopCoordinate(neighborCoords);
            
            // If neighbor is not tall enough, this tile is visible
            if (neighborHeight <= tileHeight)
            {
                allNeighborsHigherOrEqual = false;
                break;
            }
        }
        
        // Check that there is a tile above (also occluded from above)
        bool hasTileAbove = tileHeight < topCoordinate;
        
        // Occlude only if completely surrounded AND there's a tile above
        return allNeighborsHigherOrEqual && hasTileAbove;
    }
    
    /// <summary>
    /// Applies or removes occlusion from a tile
    /// </summary>
    private void ApplyOcclusion(Vector2Int hexCoords, int height, bool shouldOcclude)
    {
        if (tilemapManager == null)
            return;
        
        Vector3Int key = new Vector3Int(hexCoords.x, hexCoords.y, height);
        
        // Check if state has changed
        if (occlusionCache.TryGetValue(key, out bool cachedState))
        {
            if (cachedState == shouldOcclude)
                return; // No change
        }
        
        // Get the tile from the dictionary
        if (!tilemapManager.tiles.TryGetValue(key, out GameObject tile) || tile == null)
            return;
        
        // Disable/enable renderers
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                rend.enabled = !shouldOcclude;
            }
        }
        
        // Update cache and stats
        occlusionCache[key] = shouldOcclude;
        
        if (shouldOcclude)
        {
            totalOccludedTiles++;
            if (!cachedState) totalVisibleTiles--;
        }
        else
        {
            if (cachedState) totalOccludedTiles--;
            totalVisibleTiles++;
        }
    }
    
    /// <summary>
    /// Recalculates occlusion for all columns
    /// </summary>
    public void RecalculateAllOcclusion()
    {
        if (tilemapManager == null || tilemapManager.tiles == null)
            return;
        
        // Reset cache
        occlusionCache.Clear();
        totalOccludedTiles = 0;
        totalVisibleTiles = 0;
        
        // Get all unique column coordinates (q,r only)
        HashSet<Vector2Int> allCoords = new();
        foreach (var kvp in tilemapManager.tiles)
        {
            Vector3Int key = kvp.Key;
            allCoords.Add(new Vector2Int(key.x, key.y));
        }
        
        foreach (Vector2Int coords in allCoords)
        {
            UpdateColumnOcclusion(coords);
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"Recalculated occlusion for {allCoords.Count} columns. Occluded: {totalOccludedTiles}, Visible: {totalVisibleTiles}");
        }
    }
    
    /// <summary>
    /// Enables/disables occlusion culling
    /// </summary>
    public void SetOcclusionEnabled(bool isEnabled)
    {
        if (enableOcclusion == isEnabled)
            return;
        
        enableOcclusion = isEnabled;
        
        if (isEnabled)
        {
            RecalculateAllOcclusion();
        }
        else
        {
            // Re-enable all tiles
            foreach (var kvp in occlusionCache)
            {
                Vector3Int key = kvp.Key;
                Vector2Int hexCoords = new Vector2Int(key.x, key.y);
                int height = key.z;
                
                ApplyOcclusion(hexCoords, height, false);
            }
        }
    }
    
    // Display stats in top right
    private void OnGUI()
    {
        if (showDebugInfo && enableOcclusion)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.alignment = TextAnchor.UpperRight;
            
            float width = 300f;
            float lineHeight = 25f;
            float x = Screen.width - width - 10f;
            float y = 80f;
            
            int totalTiles = totalOccludedTiles + totalVisibleTiles;
            float occlusionPercent = totalTiles > 0 ? (totalOccludedTiles * 100f / totalTiles) : 0f;
            
            GUI.Label(new Rect(x, y, width, lineHeight), $"Neighbor Occlusion", style);
            GUI.Label(new Rect(x, y + lineHeight, width, lineHeight), $"Visible: {totalVisibleTiles} / {totalTiles}", style);
            GUI.Label(new Rect(x, y + lineHeight * 2, width, lineHeight), $"Occluded: {totalOccludedTiles} ({occlusionPercent:F1}%)", style);
        }
    }
}

