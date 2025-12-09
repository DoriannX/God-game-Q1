using System;
using UnityEngine;
using System.Collections.Generic;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private TilePool tilePool;
    [SerializeField] private BrushSizeManager brushSizeManager;
    [SerializeField] private TileSelector tileSelector;

    [SerializeField] private float tileHeight = 0.2f;
    [SerializeField] private float hexSize = 1f;
    [SerializeField] private int maxHeight = 10;

    [SerializeField] private float clickCooldown = 0.1f;

    private Camera mainCamera;
    private Vector2 currentMousePosition;
    private float lastClickTime;
    private float lastRightClickTime;

    public Dictionary<Vector3Int, GameObject> tiles { get; private set; } = new();
    private Dictionary<Vector2Int, int> columnHeights = new();
    public event Action<Vector3Int> columnModified;
    public event Action startedPlacingTiles;
    public event Action endedPlacingTiles;

    public Vector3Int currentHexCoordinates { get; private set; }

    public static TilemapManager instance { get; private set; }

    public event Action<Vector3Int> cellChanged;

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
            worldPosition = rayHit.point;

            //Debug.DrawRay(mouseRay.origin, mouseRay.direction * rayHit.distance, Color.green, 2f);
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(mouseRay, out rayDistance))
            {
                worldPosition = mouseRay.GetPoint(rayDistance);

                //Debug.DrawRay(mouseRay.origin, mouseRay.direction * rayDistance, Color.cyan, 2f);
            }
            else
            {
                //Debug.DrawRay(mouseRay.origin, mouseRay.direction * 100f, Color.red, 2f);
                //Debug.LogWarning("Could not calculate world position for mouse cursor");
                return;
            }
        }

        currentHexCoordinates = WorldToHexAxial(worldPosition);
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
    /// Places tiles in brush area at the current mouse hex coordinates and updates the occlusion culling.
    /// </summary>
    /// <param name="tilePrefab"> The tile to place </param>
    public void PlaceTiles(Material[] tileMaterials)
    {
        if(Time.time - lastClickTime < clickCooldown) // Simple click cooldown to prevent multiple placements on a single click
        {
            return;
        }
        lastClickTime = Time.time;
        var brushArea = brushSizeManager.GetBrushArea(currentHexCoordinates);
        startedPlacingTiles?.Invoke();
        
        foreach (var hexCoordinate in brushArea)
        {
            int columnHeight = GetColumnHeight(hexCoordinate);
            Vector3Int tilePosition = new Vector3Int(hexCoordinate.x, hexCoordinate.y, columnHeight + 1);
            SpawnTileAt(tilePosition, tileMaterials);
        }

        endedPlacingTiles?.Invoke();
    }

    /// <summary>
    /// Places a tile at the specified hexagonal coordinates, replacing any existing tile and updating occlusion culling
    /// </summary>
    /// <param name="hexCoordinates"> The coordinates in the hexagonal tilemap space </param>
    /// <param name="tilePrefab"> The tile to spawn </param>
    private void SpawnTileAt(Vector3Int hexCoordinates, Material[] tileMaterials)
    {
        if(hexCoordinates.z >= maxHeight) // Prevents the tile from exceeding the maximum height
        {
            return;
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
        }

        Vector3 spawnPosition = HexAxialToWorld(hexCoordinates);

        GameObject newTile = tilePool.GetTile();
        newTile.transform.position = spawnPosition;
        newTile.transform.rotation = Quaternion.identity;
        newTile.GetComponent<Renderer>().sharedMaterials = tileMaterials;
        newTile.name = $"Tile_({hexCoordinates.x}, {hexCoordinates.y}, {hexCoordinates.z})";

        tiles.Add(hexCoordinates, newTile);
        
        ChunkManager.Instance.AddGameObjectToChunk(newTile.transform.position, newTile);

        Vector2Int columnKey = new Vector2Int(hexCoordinates.x, hexCoordinates.y);
        
        if (!columnHeights.ContainsKey(columnKey) || columnHeights[columnKey] < hexCoordinates.z) 
            // Add the tile to a separate dictionary to keep track of the column heights for better calculation performance
        {
            columnHeights[columnKey] = hexCoordinates.z;
        }

        columnModified.Invoke(hexCoordinates);

        /*if (occlusionCullingManager != null)
        {
            occlusionCullingManager.UpdateOcclusionForColumn(hexCoordinates);
        }*/

        cellChanged?.Invoke(new Vector3Int(hexCoordinates.x, 0, hexCoordinates.y));
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

        return RoundToHexCoordinates(hexColumn, hexRow);
    }

    /// <summary>
    /// Round fractional hex coordinates to nearest hex axial coordinates.
    /// </summary>
    /// <param name="fractionalHexColumn"> x coordinate </param>
    /// <param name="fractionalHexRow"> y coordinate </param>
    /// <returns></returns>
    private Vector3Int RoundToHexCoordinates(float fractionalHexColumn, float fractionalHexRow)
    {
        float cubicS = -fractionalHexColumn - fractionalHexRow;

        int roundedHexColumn = Mathf.RoundToInt(fractionalHexColumn);
        int roundedHexRow = Mathf.RoundToInt(fractionalHexRow);
        int roundedCubicS = Mathf.RoundToInt(cubicS);

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

        return new Vector3Int(roundedHexColumn, roundedHexRow, 0); // Ignore z coordinate for axial representation because it is used for height levels
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
    public int GetColumnHeight(Vector2Int hexCoordinates)
    {
        return columnHeights.GetValueOrDefault(hexCoordinates, 0);
    }

    /// <summary>
    /// Removes tiles in brush area at the current mouse hex coordinates and updates the occlusion culling.
    /// </summary>
    public void RemoveTile()
    {
        if(Time.time - lastClickTime < clickCooldown)
        {
            return;
        }
        lastClickTime = Time.time;
        var brushArea = brushSizeManager.GetBrushArea(currentHexCoordinates);
        
        startedPlacingTiles.Invoke();
        
        /*bool useBatchMode = brushArea.Length > 1 && occlusionCullingManager != null;
        if (useBatchMode)
        {
            occlusionCullingManager.BeginBatch();
        }*/
        
        foreach (var hexCoordinate in brushArea)
        {
            int columnHeight = GetColumnHeight(hexCoordinate);
            if (columnHeights.ContainsKey(hexCoordinate))
            {
                RemoveTileAt(new Vector3Int(hexCoordinate.x, hexCoordinate.y, columnHeight));
            }
        }
        
        /*if (useBatchMode)
        {
            occlusionCullingManager.EndBatch();
        }*/
        
        endedPlacingTiles?.Invoke();
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

        Vector2Int columnKey = new Vector2Int(hexCoordinates.x, hexCoordinates.y);
        
        if (columnHeights.TryGetValue(columnKey, out int currentMaxHeight))
        {
            if (hexCoordinates.z == currentMaxHeight)
            {
                int newMaxHeight = -1;
                for (int heightLevel = hexCoordinates.z - 1; heightLevel >= 0; heightLevel--)
                {
                    if (tiles.ContainsKey(new Vector3Int(hexCoordinates.x, hexCoordinates.y, heightLevel)))
                    {
                        newMaxHeight = heightLevel;
                        break;
                    }
                }

                if (newMaxHeight == -1)
                {
                    columnHeights.Remove(columnKey);
                }
                else
                {
                    columnHeights[columnKey] = newMaxHeight;
                }
            }
        }
        
        columnModified?.Invoke(hexCoordinates);
        
        /*if (occlusionCullingManager != null)
        {
            occlusionCullingManager.UpdateOcclusionForColumn(hexCoordinates);
        }*/

        cellChanged?.Invoke(new Vector3Int(hexCoordinates.x, 0, hexCoordinates.y));
    }
}

