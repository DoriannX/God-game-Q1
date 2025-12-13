using System;
using UnityEngine;
using System.Collections.Generic;
using Components;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private TilePool tilePool;
    [SerializeField] private TileSelector tileSelector;

    [field: SerializeField] public float tileHeight { get; private set; } = 0.2f;
    [SerializeField] private float hexSize = 1f;
    [SerializeField] private int maxHeight = 10;

    [SerializeField] private float clickCooldown = 0.1f;

    private Camera mainCamera;
    private Vector2 currentMousePosition;
    private float lastClickTime;
    private float lastRightClickTime;

    public Dictionary<Vector3Int, GameObject> tiles { get; } = new();
    private Dictionary<Vector2Int, int> columnTopCoordinate = new();
    private Dictionary<Vector3Int, PosableObject> placedObjects = new();
    private Dictionary<GameObject, bool> prefabHasWaterSystem = new();
    private HashSet<Vector3Int> waterTilePositions = new(); // Tracks positions with water tiles for O(1) lookups
    public event Action<Vector3Int> columnModified;
    public event Action startedPlacingTiles;
    public event Action endedPlacingTiles;

    public Vector3Int currentHexCoordinates { get; private set; }

    public static TilemapManager instance { get; private set; }

    public event Action<Vector3Int> tilePlaced;
    public event Action<Vector3Int> tileRemoved;

    public BoundsInt cellBounds
    {
        get
        {
            if (tiles.Count == 0)
                return new BoundsInt(Vector3Int.zero, new Vector3Int(0, 0, 0));

            int minHexColumn = int.MaxValue;
            int maxHexColumn = int.MinValue;
            int minHexRow = int.MaxValue;
            int maxHexRow = int.MinValue;

            foreach (var hexCoordinates in tiles.Keys)
            {
                if (hexCoordinates.x < minHexColumn) minHexColumn = hexCoordinates.x;
                if (hexCoordinates.x > maxHexColumn) maxHexColumn = hexCoordinates.x;
                if (hexCoordinates.y < minHexRow) minHexRow = hexCoordinates.y;
                if (hexCoordinates.y > maxHexRow) maxHexRow = hexCoordinates.y;
            }

            Vector3 minWorldPosition = HexAxialToWorld(new Vector3Int(minHexColumn, minHexRow, 0));
            Vector3 maxWorldPosition = HexAxialToWorld(new Vector3Int(maxHexColumn, maxHexRow, 0));

            Vector3Int minBounds = new Vector3Int(
                Mathf.FloorToInt(minWorldPosition.x - hexSize),
                0,
                Mathf.FloorToInt(minWorldPosition.z - hexSize)
            );

            Vector3Int maxBounds = new Vector3Int(
                Mathf.CeilToInt(maxWorldPosition.x + hexSize),
                0,
                Mathf.CeilToInt(maxWorldPosition.z + hexSize)
            );

            Vector3Int boundsSize = maxBounds - minBounds;

            return new BoundsInt(minBounds, boundsSize);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found in the scene!");
        }

        if (tilePool == null)
        {
            Debug.LogError("TilePool is not assigned to TilemapManager!");
        }
    }

    private void Update()
    {
        UpdateMouseHexCoordinates();
    }

    /// <summary>
    /// Converts the mouse screen position to hexagonal tilemap world coordinates to get the next placement of the tile.
    /// </summary>
    private void UpdateMouseHexCoordinates()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(currentMousePosition);
        RaycastHit rayHit;

        Vector3 worldPosition;

        if (Physics.Raycast(mouseRay, out rayHit))
        {
            worldPosition = rayHit.collider.transform.position;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(mouseRay, out rayDistance))
            {
                worldPosition = mouseRay.GetPoint(rayDistance);
                DrawDebugCross(worldPosition, 0.5f, Color.cyan);
            }
            else
            {
                return;
            }
        }

        currentHexCoordinates = WorldToHexAxial(worldPosition);
    }

    private void DrawDebugCross(Vector3 center, float size, Color color)
    {
        float halfSize = size / 2f;
        Debug.DrawLine(center + Vector3.left * halfSize, center + Vector3.right * halfSize, color);
        Debug.DrawLine(center + Vector3.forward * halfSize, center + Vector3.back * halfSize, color);
        Debug.DrawLine(center + Vector3.up * halfSize, center + Vector3.down * halfSize, color);
    }

    /// <summary>
    /// Updates the mouse position from the input to be used for tile placement.
    /// </summary>
    /// <param name="mouseScreenPosition"> The current mouse screen position  </param>
    public void SetMousePos(Vector2 mouseScreenPosition)
    {
        currentMousePosition = mouseScreenPosition;
    }

    /// <summary>
    /// Checks if a prefab has a WaterSystem component (cached for performance)
    /// </summary>
    /// <param name="prefab">The prefab to check</param>
    /// <returns>True if the prefab has a WaterSystem component</returns>
    private bool PrefabHasWaterSystem(GameObject prefab)
    {
        if (!prefabHasWaterSystem.TryGetValue(prefab, out bool hasWaterSystem))
        {
            hasWaterSystem = prefab.GetComponent<WaterComponent>() != null;
            prefabHasWaterSystem[prefab] = hasWaterSystem;
        }

        return hasWaterSystem;
    }

    /// <summary>
    /// Places tiles in brush area at the current mouse hex coordinates and updates the occlusion culling.
    /// </summary>
    /// <param name="tilePrefab"> The tile to place </param>
    public void PlaceTilesAtMouse(GameObject prefab)
    {
        if (Time.time - lastClickTime <
            clickCooldown) // Simple click cooldown to prevent multiple placements on a single click
        {
            return;
        }

        lastClickTime = Time.time;
        var brushArea = BrushSizeManager.instance.GetBrushArea(currentHexCoordinates);
        startedPlacingTiles?.Invoke();

        foreach (var hexCoordinate in brushArea)
        {
            int topCoordinate = GetColumnTopCoordinate(hexCoordinate);
            Vector3Int tilePosition = new Vector3Int(hexCoordinate.x, hexCoordinate.y, topCoordinate + 1);
            SpawnTileAt(tilePosition, prefab);
        }

        endedPlacingTiles?.Invoke();
    }

    /// <summary>
    /// Places a tile at the specified hexagonal coordinates, replacing any existing tile and updating occlusion culling
    /// </summary>
    /// <param name="hexCoordinates"> The coordinates in the hexagonal tilemap space </param>
    /// <param name="tileData"> The tile data associated with the tile </param>
    public void SpawnTileAt(Vector3Int hexCoordinates, GameObject prefab)
    {
        if (hexCoordinates.z >= maxHeight) // Prevents the tile from exceeding the maximum height
        {
            return;
        }

        bool isWaterPrefab = PrefabHasWaterSystem(prefab);

        switch (!isWaterPrefab)
        {
            case true when placedObjects.ContainsKey(hexCoordinates):
            {
                var newPosition = hexCoordinates + new Vector3Int(0, 0, 1);
                placedObjects[newPosition] = placedObjects[hexCoordinates];
                placedObjects.Remove(hexCoordinates);
                placedObjects[newPosition].transform.position = HexAxialToWorld(newPosition);
                break;
            }
            case false when placedObjects.ContainsKey(hexCoordinates):
            {
                Destroy(placedObjects[hexCoordinates].gameObject);
                placedObjects.Remove(hexCoordinates);
                break;
            }
        }

        if (tiles.ContainsKey(hexCoordinates)) // Check if a tile already exists at the specified coordinates
            // because otherwise the tile will be replaced without being destroyed
        {
            GameObject existingTile = tiles[hexCoordinates];

            if (existingTile != null)
            {
                tilePool.ReleaseTile(existingTile);
            }

            tiles.Remove(hexCoordinates);
            // Remove from water positions if the existing tile was water
            waterTilePositions.Remove(hexCoordinates);
        }

        Vector3 spawnPosition = HexAxialToWorld(hexCoordinates);


        GameObject newTile = tilePool.GetTile(prefab);
        if (newTile == null)
        {
            Debug.LogError("Failed to get tile from pool!");
            return;
        }

        newTile.transform.position = spawnPosition;
        newTile.transform.rotation = Quaternion.identity;
        newTile.name = $"Tile_({hexCoordinates.x}, {hexCoordinates.y}, {hexCoordinates.z})";

        tiles.Add(hexCoordinates, newTile);

        // Track water tile positions for efficient water-on-water checks
        if (isWaterPrefab)
        {
            waterTilePositions.Add(hexCoordinates);
        }

        Vector2Int columnKey = new Vector2Int(hexCoordinates.x, hexCoordinates.y);

        if (!columnTopCoordinate.TryAdd(columnKey, 1))
        {
            columnTopCoordinate[columnKey] = columnTopCoordinate[columnKey] < hexCoordinates.z ? hexCoordinates.z : columnTopCoordinate[columnKey] ;
        }

        columnModified?.Invoke(hexCoordinates);

        tilePlaced?.Invoke(hexCoordinates);
    }

    /// <summary>
    ///  Tries to spawn an object at the current mouse hex coordinates if the underlying tile is allowed.
    /// </summary>
    /// <param name="objectPrefab"></param>
    /// <returns></returns>
    public bool TrySpawnObjectAtMouse(PosableObject objectPrefab)
    {
        if (Time.time - lastClickTime <
            clickCooldown) // Simple click cooldown to prevent multiple placements on a single click
        {
            return false;
        }

        lastClickTime = Time.time;
        SpawnObjectAtMouse(objectPrefab);
        return true;
    }

    /// <summary>
    ///  Tries to spawn an entity at the current mouse hex coordinates if the underlying tile is allowed.
    /// </summary>
    /// <param name="entityPrefab"></param>
    /// <returns></returns>
    public bool TrySpawnEntityAtMouse(PosableEntity entityPrefab)
    {
        if (Time.time - lastClickTime <
            clickCooldown) // Simple click cooldown to prevent multiple placements on a single click
        {
            return false;
        }

        lastClickTime = Time.time;
        SpawnEntityAtMouse(entityPrefab);
        return true;
    }

    /// <summary>
    ///  Tries to remove an entity at the current mouse hex coordinates.
    /// </summary>
    /// <returns> Whether it actually removed the entity or not </returns>
    public bool TryRemoveEntityAtMouse()
    {
        if (Time.time - lastClickTime <
            clickCooldown) // Simple click cooldown to prevent multiple placements on a single click
        {
            return false;
        }

        lastClickTime = Time.time;
        RemoveEntityAtMouse();
        return true;
    }

    /// <summary>
    ///  Removes an object at the current mouse hex coordinates.
    /// </summary>
    private void RemoveEntityAtMouse()
    {
        var brushArea = BrushSizeManager.instance.GetBrushArea(currentHexCoordinates);

        foreach (var hexCoordinate in brushArea)
        {
            int topCoordinate = GetColumnTopCoordinate(hexCoordinate);
            Vector3Int tilePosition = new Vector3Int(hexCoordinate.x, hexCoordinate.y, topCoordinate + 1);

            if (placedObjects.TryGetValue(tilePosition, out var posableObject))
            {
                Destroy(posableObject.gameObject);
                placedObjects.Remove(tilePosition);
            }
        }
    }

    public void SpawnDestructionAtMouse(DestructionObject objectPrefab)
    {
        SpawnPosableAtMouse(objectPrefab, false);
    }

    /// <summary>
    ///  Spawns an object at the current mouse hex coordinates.
    /// </summary>
    public void SpawnObjectAtMouse(PosableObject objectPrefab)
    {
        SpawnPosableAtMouse(objectPrefab, true);
    }

    /// <summary>
    ///  Spawns an entity at the current mouse hex coordinates.
    /// </summary>
    public void SpawnEntityAtMouse(PosableEntity entityPrefab)
    {
        SpawnPosableAtMouse(entityPrefab, false);
    }

    /// <summary>
    ///  Spawns a posable (object or entity) at the current mouse hex coordinates.
    /// </summary>
    /// <param name="posablePrefab">The posable prefab to spawn</param>
    /// <param name="storeInDictionary">Whether to store the spawned posable in the placedObjects dictionary</param>
    private void SpawnPosableAtMouse(Posable posablePrefab, bool storeInDictionary)
    {
        var brushArea = BrushSizeManager.instance.GetBrushArea(currentHexCoordinates);

        foreach (var hexCoordinate in brushArea)
        {
            int topCoordinate = GetColumnTopCoordinate(hexCoordinate);
            Vector3Int tilePosition = new Vector3Int(hexCoordinate.x, hexCoordinate.y, topCoordinate);
            GameObject tileAtPosition = GetTile(tilePosition);
            if (tileAtPosition != null)
            {
                tilePosition.z += 1; // Place on top of the tile
                // Check if trying to place a PosableObject and one already exists at this position
                if (posablePrefab is PosableObject && placedObjects.ContainsKey(tilePosition))
                {
                    continue; // Skip this position
                }

                Vector3 spawnPosition = HexAxialToWorld(tilePosition);
                spawnPosition.y += tileHeight / 2f; // Adjust Y position to sit on top of the tile
                
                
                GameObject tilePrefab = tilePool.GetOriginalPrefab(tileAtPosition);

                if (tilePrefab != null && posablePrefab.allowedTiles.Contains(tilePrefab))
                {
                    Posable newPosable = Instantiate(posablePrefab, spawnPosition, Quaternion.identity);
                    string typeName = storeInDictionary ? "Object" : "Entity";
                    newPosable.name =
                        $"{typeName}_({currentHexCoordinates.x}, {currentHexCoordinates.y}, {currentHexCoordinates.z})";

                    if (storeInDictionary && newPosable is PosableObject posableObject)
                    {
                        placedObjects[tilePosition] = posableObject;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Converts world position to hexagonal axial coordinates.
    /// </summary>
    public Vector3Int WorldToHexAxial(Vector3 worldPosition)
    {
        float hexInternalRadius = hexSize / 2f;

        float worldX = worldPosition.x;
        float worldZ = worldPosition.z;

        float hexColumn = (2f / 3f * worldX) / hexInternalRadius;
        float hexRow = (-1f / 3f * worldX + Mathf.Sqrt(3f) / 3f * worldZ) / hexInternalRadius;
        float heightLevel = worldPosition.y / tileHeight;

        return RoundToHexCoordinates(hexColumn, hexRow, heightLevel);
    }

    /// <summary>
    /// Round fractional hex coordinates to nearest hex axial coordinates.
    /// </summary>
    /// <param name="fractionalHexColumn"> x coordinate </param>
    /// <param name="fractionalHexRow"> y coordinate </param>
    /// <param name"heightLevel"> z coordinate </param>
    /// <returns></returns>
    private Vector3Int RoundToHexCoordinates(float fractionalHexColumn, float fractionalHexRow, float heightLevel)
    {
        float cubicS = -fractionalHexColumn - fractionalHexRow;

        int roundedHexColumn = Mathf.RoundToInt(fractionalHexColumn);
        int roundedHexRow = Mathf.RoundToInt(fractionalHexRow);
        int roundedCubicS = Mathf.RoundToInt(cubicS);
        int roundedHeightLevel = Mathf.RoundToInt(heightLevel);

        float columnRoundingError = Mathf.Abs(roundedHexColumn - fractionalHexColumn);
        float rowRoundingError = Mathf.Abs(roundedHexRow - fractionalHexRow);
        float cubicSRoundingError = Mathf.Abs(roundedCubicS - cubicS);


        if (columnRoundingError > rowRoundingError && columnRoundingError > cubicSRoundingError)
        {
            roundedHexColumn = -roundedHexRow - roundedCubicS;
        }
        else if (rowRoundingError > cubicSRoundingError)
        {
            roundedHexRow = -roundedHexColumn - roundedCubicS;
        }

        return new Vector3Int(roundedHexColumn, roundedHexRow, roundedHeightLevel);
    }

    /// <summary>
    /// Converts hexagonal axial coordinates to world position.
    /// </summary>
    public Vector3 HexAxialToWorld(Vector3Int hexCoordinates)
    {
        float hexInternalRadius = hexSize / 2f;

        float worldX = hexInternalRadius * (3f / 2f * hexCoordinates.x);
        float worldZ = hexInternalRadius * (Mathf.Sqrt(3f) / 2f * hexCoordinates.x + Mathf.Sqrt(3f) * hexCoordinates.y);

        float worldY = hexCoordinates.z * tileHeight;

        //The y coordinate is calculated based on the height level of the tile
        return new Vector3(worldX, worldY, worldZ);
    }

    /// <summary>
    /// Returns the tile GameObject at the specified hexagonal coordinates.
    /// </summary>
    public GameObject GetTile(Vector3Int cellPos)
    {
        tiles.TryGetValue(cellPos, out GameObject tile);
        return tile;
    }

    /// <summary>
    /// Returns the current height of the column at the specified hexagonal axial coordinates to place the next tile correctly.
    /// </summary>
    /// <param name="hexCoordinates"> The coordinates in 2D space of the column </param>
    public int GetColumnTopCoordinate(Vector2Int hexCoordinates)
    {
        if (columnTopCoordinate.TryGetValue(hexCoordinates, out int height))
        {
            return height;
        }

        columnTopCoordinate[hexCoordinates] = 0;
        return 0;
    }

    /// <summary>
    /// Removes tiles in brush area at the current mouse hex coordinates and updates the occlusion culling.
    /// </summary>
    public void RemoveTileAtMouse()
    {
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        lastClickTime = Time.time;
        var brushArea = BrushSizeManager.instance.GetBrushArea(currentHexCoordinates);

        startedPlacingTiles?.Invoke();

        /*bool useBatchMode = brushArea.Length > 1 && occlusionCullingManager != null;
        if (useBatchMode)
        {
            occlusionCullingManager.BeginBatch();
        }*/

        foreach (var hexCoordinate in brushArea)
        {
            int topCoordinate = GetColumnTopCoordinate(hexCoordinate);
            RemoveTileAt(new Vector3Int(hexCoordinate.x, hexCoordinate.y, topCoordinate));
        }

        /*if (useBatchMode)
        {
            occlusionCullingManager.EndBatch();
        }*/

        endedPlacingTiles?.Invoke();
    }

    /// <summary>
    /// Removes non-water tiles in brush area at the current mouse hex coordinates. Skips water tiles.
    /// </summary>
    public void RemoveTileAtMouseExceptWater()
    {
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        lastClickTime = Time.time;
        var brushArea = BrushSizeManager.instance.GetBrushArea(currentHexCoordinates);

        startedPlacingTiles?.Invoke();

        /*bool useBatchMode = brushArea.Length > 1 && occlusionCullingManager != null;
        if (useBatchMode)
        {
            occlusionCullingManager.BeginBatch();
        }*/

        foreach (var hexCoordinate in brushArea)
        {
            int topCoordinate = GetColumnTopCoordinate(hexCoordinate);
            Vector3Int tilePosition = new Vector3Int(hexCoordinate.x, hexCoordinate.y, topCoordinate);

            // Skip water tiles using O(1) HashSet lookup
            if (!waterTilePositions.Contains(tilePosition))
            {
                RemoveTileAt(tilePosition);
            }
        }

        /*if (useBatchMode)
        {
            occlusionCullingManager.EndBatch();
        }*/

        endedPlacingTiles?.Invoke();
    }

    /// <summary>
    ///  Removes all water tiles at the current mouse hex coordinates.
    /// </summary>
    public void RemoveAllWaterAtMouse()
    {
        if (Time.time - lastClickTime < clickCooldown)
        {
            return;
        }

        lastClickTime = Time.time;
        Vector2Int[] brushArea = BrushSizeManager.instance.GetBrushArea(currentHexCoordinates);
        foreach (var hexCoordinate in brushArea)
        {
            RemoveAllWaterAt(new Vector3Int(hexCoordinate.x, hexCoordinate.y, 0));
        }
    }


    /// <summary>
    ///  Removes all water tiles at the specified hexagonal coordinates.
    /// </summary>
    /// <param name="hexCoordinates"> The coordinates in the hexagonal tilemap space </param>
    public void  RemoveAllWaterAt(Vector3Int hexCoordinates)
    {
        print("Removing all water at: " + hexCoordinates);
        int topCoordinate = GetColumnTopCoordinate(new Vector2Int(hexCoordinates.x, hexCoordinates.y));

        for (int z = topCoordinate; z >= 0; z--)
        {
            Vector3Int tilePosition = new Vector3Int(hexCoordinates.x, hexCoordinates.y, z);
            if (waterTilePositions.Contains(tilePosition))
            {
                RemoveTileAt(tilePosition);
            }
            else
            {
                break; // Stop if a non-water tile is encountered
            }
        }
    }

    /// <summary>
    ///  Removes the tile at the specified hexagonal coordinates, updates column height and occlusion culling.
    /// </summary>
    /// <param name="hexCoordinates"></param>
    public void RemoveTileAt(Vector3Int hexCoordinates)
    {
        if (!tiles.TryGetValue(hexCoordinates, out var tileToRemove))
            return;

        if (tileToRemove != null)
        {
            tilePool.ReleaseTile(tileToRemove);
        }

        tiles.Remove(hexCoordinates);

        // Remove from water positions set if it was a water tile
        waterTilePositions.Remove(hexCoordinates);

        Vector2Int columnKey = new Vector2Int(hexCoordinates.x, hexCoordinates.y);
        columnTopCoordinate[columnKey] = hexCoordinates.z - 1;
        if (columnTopCoordinate[columnKey] < 0)
        {
            columnTopCoordinate.Remove(columnKey);
        }

        columnModified?.Invoke(hexCoordinates);

        /*if (occlusionCullingManager != null)
        {
            occlusionCullingManager.UpdateOcclusionForColumn(hexCoordinates);
        }*/

        tileRemoved?.Invoke(hexCoordinates);
    }

    /// <summary>
    /// Gets the placed object at the specified position, if any.
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>The PosableObject at that position, or null if none exists</returns>
    public PosableObject GetPlacedObjectAt(Vector3Int position)
    {
        placedObjects.TryGetValue(position, out var placedObject);
        return placedObject;
    }

    /// <summary>
    /// Removes a placed object at the specified position from the dictionary.
    /// Does NOT destroy the GameObject - caller is responsible for that.
    /// </summary>
    /// <param name="position">The position to remove from</param>
    /// <returns>True if an object was removed, false otherwise</returns>
    public bool RemovePlacedObjectAt(Vector3Int position)
    {
        return placedObjects.Remove(position);
    }
    
    public Vector3 GetHexCellSize()
    {
        float width = hexSize;
        float height = hexSize * Mathf.Sqrt(3f);

        return new Vector3(width, 0f, height);
    }
}