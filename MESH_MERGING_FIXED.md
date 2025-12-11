# Mesh Merging Fix - Visibility Issue

## Changes Made

The `MapGenerator.cs` has been updated to properly merge tile meshes into a single visible mesh. Here are the key improvements:

### 1. **Enhanced Mesh Collection**
- Now searches for `MeshFilter` components in children of tiles (using `GetComponentsInChildren`)
- This ensures we find meshes even if they're nested in child GameObjects

### 2. **Improved Debugging**
- Added extensive debug logging to help identify issues:
  - Total meshes found
  - Material information
  - Bounds of combined mesh
  - Vertex and triangle counts
  - Renderer state (enabled/disabled)
  - Material assignment status

### 3. **Better Mesh Settings**
- Explicitly enables the MeshRenderer on merged mesh
- Sets shadow casting mode to `On`
- Sets receive shadows to `true`
- Handles large meshes by setting `IndexFormat.UInt32` when needed

### 4. **Validation**
- Checks if combined mesh has vertices before assigning it
- Validates materials exist before using them
- Skips invalid or null components

### 5. **New Options**
- `addColliderToMergedMesh`: Add a MeshCollider to the merged mesh (useful for raycasting)
- `destroyOriginalTiles`: Completely remove original tile GameObjects after merging

## Debugging Steps

If the merged mesh is still not visible, check these things in Unity:

### 1. Check the Console Logs
Look for these debug messages:
```
Found X meshes to combine across Y material(s)
Created submesh 'Submesh_MaterialName' with X meshes
  - Material: MaterialName
  - Bounds: (...)
  - Vertex count: X
  - Triangle count: X
  - Renderer enabled: True
  - Material assigned: True
```

### 2. Check the Scene Hierarchy
- Look for a GameObject named `MergedTileMesh`
- Expand it to see submeshes (e.g., `Submesh_TileMaterial`)
- Make sure these objects are active in the hierarchy

### 3. Check the Inspector
Select the submesh object and verify:
- **MeshFilter**: Has a mesh assigned
- **MeshRenderer**: 
  - Is enabled (checkbox is checked)
  - Has a material assigned
  - Material shader is rendering correctly
- **Transform**: Position should be at or near world origin (0,0,0)

### 4. Camera Culling
- Make sure your camera can see the bounds of the merged mesh
- In Scene view, press `F` while selecting the merged mesh to frame it
- Check if the mesh bounds are correct (not at 0,0,0 with size 0)

### 5. Layer and Culling Mask
- Check if the merged mesh is on a layer that your camera can see
- Verify the camera's culling mask includes the layer

### 6. Material Issues
- Check if the material shader is compatible with your render pipeline
- Try assigning a simple Standard shader material to test
- Verify the material is not transparent or has alpha set to 0

## How to Use

1. In Unity, select the GameObject with the `MapGenerator` component
2. Configure the settings:
   - `Map Width`: Width of the map (number of tiles)
   - `Map Height`: Height of the map (number of tiles)
   - `Layout Type`: Use `OddROffset` for flat-top hex in a rectangle
   - `Merge Meshes`: Enable to combine all tiles into one mesh
   - `Remove Colliders`: Enable to remove colliders from individual tiles
   - `Add Collider To Merged Mesh`: Enable to add a MeshCollider to merged mesh
   - `Destroy Original Tiles`: Enable to completely remove original tiles after merging

3. Run the game and check the console for debug messages

## Common Issues and Solutions

### Issue: "No meshes found to merge"
**Solution**: Your tile prefabs don't have MeshFilter/MeshRenderer components. Check that the tiles spawned by TilemapManager have mesh components.

### Issue: Mesh appears but at wrong location
**Solution**: The merge uses world space transforms, so the position should be correct. Check if the original tiles are at the right positions.

### Issue: Mesh is black or has wrong shading
**Solution**: 
- Normals might be inverted. Try selecting the mesh and using Unity's "Recalculate Normals" option.
- The material might need specific setup. Copy the material settings from an original tile.

### Issue: Mesh disappears when camera moves
**Solution**: The bounds might be incorrect. Try manually setting larger bounds on the merged mesh in code.

### Issue: Performance is still slow
**Solution**: 
- Enable `destroyOriginalTiles` to completely remove the original tiles
- Consider using GPU instancing if tiles share the same material
- Split the merged mesh into smaller chunks if it's too large

## Next Steps

If the merged mesh is still not visible after these changes:
1. Check the debug logs in the Console
2. Verify the MergedTileMesh GameObject exists in the hierarchy
3. Try enabling `destroyOriginalTiles` to see if original tiles are interfering
4. Check if the camera can actually see the bounds of the merged mesh
5. Try assigning a simple material (like Unity's Default-Material) to the merged mesh manually in the Inspector

