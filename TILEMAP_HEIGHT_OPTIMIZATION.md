# TilemapManager Height System Optimization

## Date
December 5, 2025

## Problem
The original implementation was **extremely inefficient** when getting column heights:
- **O(n) complexity** - iterated through ALL tiles every time `GetColumnHeight()` was called
- With thousands of tiles, this caused massive performance issues
- When placing many tiles per tick (brush painting), this multiplied the problem

## Solution
Added a separate `columnHeights` dictionary to track the maximum height of each column.

### New Data Structure
```csharp
private Dictionary<Vector2Int, int> columnHeights = new();
```
- **Key**: `Vector2Int(q, r)` - the hex coordinates
- **Value**: `int` - the next available height (current max height + 1)

## Performance Improvement

### Before (O(n))
```csharp
public int GetColumnHeight(int q, int r)
{
    int maxZ = 0;
    foreach (var kvp in tiles) // Iterates through ALL tiles
    {
        Vector3Int key = kvp.Key;
        if (key.x == q && key.y == r)
        {
            if (key.z >= maxZ)
            {
                maxZ = key.z + 1;
            }
        }
    }
    return maxZ;
}
```
- **Complexity**: O(n) where n = total number of tiles in the scene
- **Example**: With 10,000 tiles, checking 100 columns = 1,000,000 comparisons

### After (O(1))
```csharp
public int GetColumnHeight(int q, int r)
{
    Vector2Int key = new Vector2Int(q, r);
    return columnHeights.TryGetValue(key, out int height) ? height : 0;
}
```
- **Complexity**: O(1) - constant time dictionary lookup
- **Example**: With 10,000 tiles, checking 100 columns = 100 lookups

## Speed Improvement
- **Theoretical**: 10,000x faster for 10,000 tiles
- **Practical**: Instant lookups regardless of tile count

## Implementation Details

### 1. SpawnTileAt() Updates
```csharp
// Update column height tracker
Vector2Int columnKey = new Vector2Int(hexCoords.x, hexCoords.y);
int newHeight = hexCoords.z + 1; // +1 because z is 0-indexed
if (!columnHeights.ContainsKey(columnKey) || columnHeights[columnKey] < newHeight)
{
    columnHeights[columnKey] = newHeight;
}
```
- When a tile is spawned, update the column height if necessary
- Only updates if the new tile is taller than existing height

### 2. RemoveColumn() Updates
```csharp
// Update column height tracker - remove the column entry
Vector2Int columnKey = new Vector2Int(q, r);
columnHeights.Remove(columnKey);
```
- When an entire column is removed, delete the height entry
- The column height will be recalculated if tiles are added later

## Memory Trade-off
- **Additional Memory**: ~12 bytes per unique column (Vector2Int + int)
- **Example**: 1,000 columns = ~12 KB
- **Benefit**: Massive performance improvement for negligible memory cost

## Edge Cases Handled
1. **New column**: Returns 0 (spawn at ground level)
2. **Removed column**: Entry deleted, returns 0 for next spawn
3. **Partial column removal**: Not currently supported (would need recalculation)
4. **Replace mode**: Removes entire column, resets height to 0

## Future Optimizations (if needed)
1. **Serialize columnHeights**: Save/load to avoid recalculation on scene load
2. **Partial removal support**: Track individual tile removals and recalculate height
3. **Height queries by range**: Add method to get all columns within a height range

## Testing Notes
- Place tiles with large brush sizes - should be instant now
- Build tall columns - height tracking should be accurate
- Replace columns - heights should reset correctly
- Performance should scale with number of columns, not total tiles

