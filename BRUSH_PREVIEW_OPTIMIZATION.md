# BrushPreview Optimization - Using TilemapManager Methods

## Date
December 5, 2025

## Summary
Refactored `BrushPreview.cs` to use TilemapManager's methods and calculations instead of having duplicate logic, ensuring the preview shows exactly what will be placed.

## Key Changes

### 1. Removed Duplicate Parameters
**Before:**
```csharp
[SerializeField] private float hexSize = 1f;
[SerializeField] private float tileHeight = 0.2f;
[SerializeField] private float previewHeight = 0.01f;
```

**After:**
```csharp
[SerializeField] private TilemapManager tilemapManager;
```

**Why:** The preview was using its own hex size and tile height calculations, which could desync from the actual placement logic.

### 2. Use TilemapManager's WorldToHexAxial
**Before:**
```csharp
Vector3Int centerHex = TilemapManager.instance.WorldToHexAxial(worldPosition);
```

**After:**
```csharp
Vector3Int centerHex = tilemapManager.WorldToHexAxial(worldPosition);
```

**Why:** Uses the same reference and ensures consistent coordinate conversion.

### 3. Use TilemapManager's GetColumnHeight
**Before:**
```csharp
Vector3 hexWorldPos = TilemapManager.instance.HexAxialToWorld(
    new Vector3Int(brushArea[i].x, brushArea[i].y, 
    TilemapManager.instance.GetColumnHeight(brushArea[i]))
);
```

**After:**
```csharp
Vector2Int coord = brushArea[i];
int height = tilemapManager.GetColumnHeight(coord);
Vector3Int hexCoords = new Vector3Int(coord.x, coord.y, height + 1);
Vector3 hexWorldPos = tilemapManager.HexAxialToWorld(hexCoords);
```

**Why:** 
- Matches the exact logic in `TilemapManager.DetectMouseClick()`
- Uses `height + 1` because tiles spawn on top of existing columns
- More readable and maintainable

### 4. Use TilemapManager's HexAxialToWorld
**Why:** Ensures the preview position matches exactly where the tile will be spawned, including proper height calculation.

## Preview Logic Now Matches Placement Logic

### TilemapManager.DetectMouseClick():
```csharp
foreach (var coord in brushArea)
{
    int height = GetColumnHeight(coord);
    SpawnTileAt(new Vector3Int(coord.x, coord.y, height+1), tilePrefab);
}
```

### BrushPreview.UpdatePreview():
```csharp
for (int i = 0; i < brushArea.Length; i++)
{
    Vector2Int coord = brushArea[i];
    int height = tilemapManager.GetColumnHeight(coord);
    Vector3Int hexCoords = new Vector3Int(coord.x, coord.y, height + 1);
    Vector3 hexWorldPos = tilemapManager.HexAxialToWorld(hexCoords);
    // ... create preview at hexWorldPos
}
```

**They're now identical!**

## Benefits

1. **No Desync**: Preview always shows exactly where tiles will be placed
2. **Single Source of Truth**: All hex calculations come from TilemapManager
3. **Easier Maintenance**: Change hex size or height in one place (TilemapManager)
4. **Performance**: Uses optimized `GetColumnHeight()` with O(1) dictionary lookup
5. **Consistency**: Guaranteed to match placement behavior

## Testing Checklist

- [ ] Preview appears at correct positions
- [ ] Preview shows correct height (on top of existing columns)
- [ ] Preview matches tile placement exactly (no offset)
- [ ] Changing hex size in TilemapManager updates preview automatically
- [ ] Large brush sizes show previews correctly
- [ ] Tall columns show preview at correct height
- [ ] Switching tiles updates preview with new prefab

## Technical Details

### Coordinate System
- Uses `Vector2Int(q, r)` for hex coordinates
- Height is stored in z-component: `Vector3Int(q, r, height)`
- `height = 0` is ground level
- Preview shows at `height + 1` (next available position)

### Update Strategy
- Updates on mouse move
- Updates on brush size change
- Updates on tile selection change
- Auto-updates every 0.1s to catch column height changes from other sources

### Object Pooling
- Reuses preview GameObjects when possible
- Only destroys and recreates when tile prefab changes
- Deactivates unused objects instead of destroying

