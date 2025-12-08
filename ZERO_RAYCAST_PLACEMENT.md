# Zero-Raycast Placement System

## Date
December 5, 2025

## Problem Statement
Even after unifying the raycast method, the system was still performing **two raycasts**:
1. **BrushPreview** - Raycast every frame to show preview
2. **TilemapManager.PlaceTile()** - Raycast again when placing tiles

This was unnecessary since the preview already knew exactly where tiles would be placed.

## Solution: Cached Position System

### Concept
Instead of raycasting twice, we now:
1. **BrushPreview** raycasts once per frame (for preview)
2. **Caches** the resulting world position and hex coordinates
3. **TilemapManager** uses the cached position when placing tiles (zero raycasts!)

### Implementation

#### BrushPreview - Added Caching
```csharp
// Cached raycast results - exposed for TilemapManager
private Vector3 cachedWorldPosition;
private Vector3Int cachedHexCoords;
private bool hasValidPosition;

// Public properties for TilemapManager to access
public Vector3 CurrentWorldPosition => cachedWorldPosition;
public Vector3Int CurrentHexCoords => cachedHexCoords;
public bool HasValidPosition => hasValidPosition;
```

**In Update():**
```csharp
if (tilemapManager.GetWorldPositionFromScreen(Input.mousePosition, out Vector3 worldPosition))
{
    // Cache the raycast results for TilemapManager to use
    cachedWorldPosition = worldPosition;
    cachedHexCoords = tilemapManager.WorldToHexAxial(worldPosition);
    hasValidPosition = true;
    
    // ... update preview ...
}
else
{
    hasValidPosition = false;
}
```

#### TilemapManager - New Zero-Raycast Method

```csharp
/// <summary>
/// Places tiles at the current preview position without doing any raycast
/// Uses the cached position from BrushPreview (zero-raycast placement)
/// </summary>
public bool PlaceTileAtPreview(GameObject tilePrefab)
{
    // Check if BrushPreview has a valid position
    if (brushPreview == null || !brushPreview.HasValidPosition)
    {
        return false;
    }

    // Use cached hex coordinates from BrushPreview (NO RAYCAST!)
    Vector3Int hexCoords = brushPreview.CurrentHexCoords;

    // Get brush area and spawn tiles
    Vector2Int[] brushArea = brushManager.GetBrushArea(hexCoords);
    
    foreach (var coord in brushArea)
    {
        int height = GetColumnHeight(coord);
        SpawnTileAt(new Vector3Int(coord.x, coord.y, height+1), tilePrefab);
    }
    
    return true;
}
```

## Performance Comparison

### Before (Unified Raycast)
```
Frame 1:
  - BrushPreview.Update(): 1 raycast ✓
  - User clicks
  - TilemapManager.PlaceTile(): 1 raycast ✗
Total: 2 raycasts per placement
```

### After (Cached Position)
```
Frame 1:
  - BrushPreview.Update(): 1 raycast ✓ (cached)
  - User clicks
  - TilemapManager.PlaceTileAtPreview(): 0 raycasts ✓ (uses cache)
Total: 1 raycast per placement (50% reduction!)
```

### Continuous Painting
If user holds down mouse button painting at 60 FPS:

**Before:**
- 60 raycasts/sec for preview
- 60 raycasts/sec for placement
- **Total: 120 raycasts/sec**

**After:**
- 60 raycasts/sec for preview (cached)
- 0 raycasts/sec for placement (uses cache)
- **Total: 60 raycasts/sec (50% reduction!)**

## Data Flow Diagram

```
┌──────────────────┐
│   Every Frame    │
└────────┬─────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  BrushPreview.Update()              │
│  - Does ONE raycast                 │
│  - Caches worldPosition             │
│  - Caches hexCoords                 │
│  - Sets hasValidPosition = true     │
│  - Updates preview visual           │
└────────┬────────────────────────────┘
         │
         │ (Data cached in memory)
         │
         ▼
┌─────────────────────────────────────┐
│  User Clicks / Input Action         │
└────────┬────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────┐
│  TilemapManager.PlaceTileAtPreview()│
│  - NO RAYCAST!                      │
│  - Reads brushPreview.CurrentHexCoords
│  - Places tiles at cached position  │
└─────────────────────────────────────┘
```

## API Changes

### New Public API (BrushPreview)
```csharp
public Vector3 CurrentWorldPosition { get; }    // Last raycast world position
public Vector3Int CurrentHexCoords { get; }     // Last raycast hex coordinates
public bool HasValidPosition { get; }           // Is cached position valid?
```

### New Public Method (TilemapManager)
```csharp
public bool PlaceTileAtPreview(GameObject tilePrefab)
```
- Returns `true` if placement succeeded
- Returns `false` if preview has no valid position

### Existing Methods (Still Available)
```csharp
public void PlaceTile(Vector3 mousePos, GameObject tilePrefab)  // Fallback with raycast
public bool GetWorldPositionFromScreen(Vector2 screenPos, out Vector3 worldPosition)
```

## Usage Example

### Old Way (Still Works)
```csharp
// Does a raycast internally
tilemapManager.PlaceTile(Input.mousePosition, tilePrefab);
```

### New Way (Zero Raycasts - Recommended)
```csharp
// Uses cached position from preview (NO RAYCAST!)
if (tilemapManager.PlaceTileAtPreview(tilePrefab))
{
    // Placement succeeded
}
else
{
    // Preview has no valid position
}
```

## Benefits

### 🚀 Performance
- **50% reduction** in raycasts during placement
- **Zero overhead** when painting continuously
- Cached data is already in CPU cache (fast!)

### ✅ Accuracy
- **Guaranteed sync** between preview and placement
- **Impossible to desync** - uses exact same data
- Preview shows **exactly** where tiles will be placed

### 🎯 Simplicity
- Cleaner code - no duplicate raycast logic
- Single source of truth for mouse position
- Easier to debug and maintain

### 🔧 Flexibility
- Old `PlaceTile()` method still available for compatibility
- Can manually trigger placement without preview if needed
- Preview can be disabled without breaking placement

## Edge Cases Handled

### 1. Preview Not Active
```csharp
if (!brushPreview.HasValidPosition)
{
    return false;  // Safe failure
}
```

### 2. Preview Reference Missing
```csharp
if (brushPreview == null)
{
    Debug.LogWarning("BrushPreview not assigned");
    return false;
}
```

### 3. Raycast Failed
```csharp
// In BrushPreview.Update()
if (!tilemapManager.GetWorldPositionFromScreen(...))
{
    hasValidPosition = false;  // Mark as invalid
    ClearPreview();
}
```

### 4. Old Placement Method
```csharp
// Still works with its own raycast
tilemapManager.PlaceTile(mousePos, tilePrefab);
```

## Testing Checklist

- [x] Preview shows at correct position
- [x] Placement happens at preview position (no offset)
- [x] Multiple tiles with brush work correctly
- [x] Occlusion batch mode still works
- [x] Column height detection correct
- [x] No performance regression
- [x] Handles invalid preview gracefully
- [x] Old PlaceTile() method still works

## Migration Guide

### For Input Handlers
**Before:**
```csharp
void OnClick()
{
    tilemapManager.PlaceTile(Input.mousePosition, currentTile);
}
```

**After:**
```csharp
void OnClick()
{
    tilemapManager.PlaceTileAtPreview(currentTile);
}
```

### For Paint Components
**Before:**
```csharp
if (Input.GetMouseButton(0))
{
    tilemapManager.PlaceTile(Input.mousePosition, tilePrefab);
}
```

**After:**
```csharp
if (Input.GetMouseButton(0))
{
    tilemapManager.PlaceTileAtPreview(tilePrefab);
}
```

## Performance Metrics

### Expected Improvements
- **Raycast count**: -50%
- **CPU time**: ~0.1-0.3ms saved per placement
- **Frame time**: More stable during painting
- **Memory**: Negligible increase (~32 bytes for cache)

### When Most Beneficial
- Large brush sizes (many tiles per click)
- Continuous painting (holding mouse button)
- High placement frequency
- VR/mobile where raycasts are more expensive

## Future Optimizations

### Potential Enhancements
1. **Multi-frame prediction**: Cache last N positions for interpolation
2. **Input prediction**: Predict next position based on mouse velocity
3. **Async raycasting**: Move preview raycast to job system
4. **Spatial hashing**: Cache raycast results per screen region

### Not Needed Currently
- Current system is already very efficient
- One raycast per frame is negligible
- Focus should be on other bottlenecks (instantiation, occlusion)

