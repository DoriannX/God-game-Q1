# Camera Bounds System

## Overview
The Camera Bounds System prevents the camera from moving outside the map boundaries, keeping the player's view within the playable area.

## Components

### CameraBoundsComponent
Located at: `Assets/Scripts/Components/CameraBoundsComponent.cs`

This component automatically constrains the camera position to stay within the map bounds defined by the TilemapManager.

#### Features
- **Automatic Bounds Detection**: Reads map bounds from `TilemapManager.instance.cellBounds`
- **Configurable Margin**: Add extra space beyond the map edges (default: 2 units)
- **Toggle On/Off**: Can be enabled or disabled at runtime
- **Visual Debug**: Shows bounds in the Scene view with yellow wireframe cube

#### Inspector Properties
- `Bounds Margin` (float, default: 2f): Extra padding beyond the map bounds
- `Enable Bounds` (bool, default: true): Toggle to enable/disable bounds checking

#### Usage
The component is automatically added to the camera via the `[RequireComponent]` attribute in CameraController. No manual setup required!

#### Public Methods
```csharp
// Enable or disable bounds checking
public void SetBoundsEnabled(bool isEnabled)

// Change the margin around the map bounds
public void SetBoundsMargin(float margin)
```

## Integration

The CameraBoundsComponent is automatically added to any GameObject with a CameraController component through the `[RequireComponent(typeof(CameraBoundsComponent))]` attribute.

The component works by:
1. Checking the TilemapManager for the current map bounds every frame in `LateUpdate()`
2. Clamping the camera's X and Z positions to stay within bounds + margin
3. Updating the camera position if it goes out of bounds

## How It Works

1. **Bounds Calculation**: The system uses `TilemapManager.instance.cellBounds` which returns a `BoundsInt` containing the min/max coordinates of all placed tiles
2. **Margin Application**: A configurable margin is added to all sides of the bounds
3. **Position Clamping**: In `LateUpdate()`, the camera position is clamped using `Mathf.Clamp()` for X and Z axes
4. **Y-Axis Free**: The Y-axis (height) is not constrained, allowing free zoom functionality

## Example Code

```csharp
// Access the component at runtime
CameraBoundsComponent cameraBounds = Camera.main.GetComponent<CameraBoundsComponent>();

// Disable bounds temporarily
cameraBounds.SetBoundsEnabled(false);

// Set a larger margin
cameraBounds.SetBoundsMargin(5f);

// Re-enable bounds
cameraBounds.SetBoundsEnabled(true);
```

## Notes
- The bounds automatically update as the map changes (tiles added/removed)
- If no map exists (no tiles placed), the bounds system is inactive
- The system uses `LateUpdate()` to ensure it runs after camera movement
- Works seamlessly with the existing ZoomComponent and MovementComponent

## Testing
To test the system:
1. Generate a map using MapGenerator
2. Try to move the camera beyond the map edges
3. The camera should stop at the bounds + margin
4. In Scene view, you'll see a yellow wireframe box showing the bounds
5. Check the Console for debug logs showing actual bounds values

## Troubleshooting

### Debug Logging
The component includes debug logging that shows:
- Map bounds min/max in world coordinates
- Current camera position

Check the Console output to verify:
```
Map Bounds: min=(-5, -8), max=(5, 8), Camera=(0.00, 0.00)
```

If the bounds seem too small or too large, check:
- MapGenerator map size settings
- TilemapManager hexSize value (default: 1)
- Bounds margin in CameraBoundsComponent (default: 2)

### Camera Goes Beyond Bounds
If the camera still goes beyond bounds:
1. Verify TilemapManager.instance exists
2. Check that tiles have been generated
3. Verify cellBounds size is > 0
4. Check debug logs to see actual min/max values

## Future Enhancements
Possible improvements:
- Smooth damping when hitting boundaries
- Different margins for each side (top, bottom, left, right)
- Bounds based on viewport size to keep map always visible
- Warning/notification when player tries to go out of bounds

