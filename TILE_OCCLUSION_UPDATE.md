# Tile Occlusion System Update

## Date
December 5, 2025

## Summary
Updated `TileNeighborOcclusionCulling.cs` to work with the new height system in `TilemapManager.cs` where the dictionary key structure changed to use the z-coordinate for height.

## Key Changes

### Dictionary Structure Change
**Old System:**
- Height was managed separately in a column-based system
- Required a separate `TileHeightManager` reference

**New System:**
- Dictionary key: `Vector3Int(q, r, height)` where:
  - `x` = q (hex coordinate)
  - `y` = r (hex coordinate)  
  - `z` = height (0-indexed, base tile = 0)

### Modified Methods

1. **UpdateOcclusionForColumn(Vector3Int hexCoords)**
   - Now converts Vector3Int to Vector2Int for hex coordinates
   - Works with the new dictionary structure

2. **GetColumnHeight(Vector2Int hexCoords)**
   - NEW METHOD: Queries TilemapManager.tiles dictionary
   - Finds maximum z value for given q,r coordinates
   - Returns column height (max z + 1)

3. **UpdateColumnOcclusion(Vector2Int hexCoords)**
   - Now works with Vector2Int for hex coordinates
   - Uses GetColumnHeight() to determine column size

4. **ShouldOccludeTile(Vector2Int hexCoords, int tileHeight, int columnHeight)**
   - Updated to use Vector2Int
   - Gets neighbor heights from GetColumnHeight()

5. **ApplyOcclusion(Vector2Int hexCoords, int height, bool shouldOcclude)**
   - Updated to construct key as: `Vector3Int(q, r, height)`
   - Queries tiles directly from TilemapManager.tiles dictionary

6. **RemoveColumn(Vector3Int hexCoords)**
   - Converts to Vector2Int internally
   - Uses new GetColumnHeight() method

7. **RecalculateAllOcclusion()**
   - Extracts unique (q,r) coordinates from dictionary
   - Processes each column independently

### Removed Dependencies
- No longer depends on `TileHeightManager`
- No longer needs `heightManager.GetColumnHeight()`
- No longer needs `heightManager.GetColumnTiles()`

### Code Quality Improvements
- Removed unused `using System.Linq`
- Fixed FindObjectOfType deprecation (now uses FindFirstObjectByType)
- Fixed redundant default value initializations
- Fixed variable naming conflicts (renderer → rend, enabled → isEnabled)
- Fixed comparison with true/false warnings

## How It Works

### Occlusion Logic
1. A tile is occluded if:
   - All 6 horizontal neighbors have height > tile height
   - There is a tile above it (not the top tile)
   - Occlusion is enabled for that tile type

2. Batch mode:
   - When placing many tiles, batch mode accumulates affected columns
   - Processes all at once when EndBatch() is called
   - Significantly improves performance

### Integration with TilemapManager
- TilemapManager calls `UpdateOcclusionForColumn()` when tiles are placed/removed
- Uses BeginBatch() / EndBatch() for brush operations
- Directly accesses `TilemapManager.tiles` dictionary

## Testing Recommendations
1. Place tiles in various patterns to ensure occlusion works correctly
2. Build tall columns and verify only top tile is visible when surrounded
3. Test with different brush sizes
4. Verify performance with large amounts of tiles
5. Enable showDebugInfo to monitor occlusion stats

## Performance Notes
- No raycasting (neighbor-based only)
- O(1) dictionary lookups
- Batch mode prevents redundant calculations
- Only affected columns are recalculated when tiles change

