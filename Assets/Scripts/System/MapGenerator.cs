using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum GridLayout
    {
        OddROffset,      // Odd-r: Standard horizontal rectangle for flat-top hex
        EvenROffset,     // Even-r: Alternative horizontal rectangle
        OddQOffset,      // Odd-q: Standard vertical rectangle
        EvenQOffset,     // Even-q: Alternative vertical rectangle
        Simple           // Simple diagonal (diamond shape) - for comparison
    }
    
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;
    [SerializeField] private GridLayout layoutType = GridLayout.OddROffset;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        Debug.Log($"Generating map of size {mapWidth}x{mapHeight} using {layoutType} layout");
        
        for (int x = -mapWidth/2; x < mapWidth/2; x++)
        {
            for (int y = -mapHeight/2; y < mapHeight/2; y++)
            {
                Vector3Int tilePosition = OffsetToAxial(x, y, layoutType);
                TilemapManager.instance.SpawnTileAt(tilePosition, TileSelector.instance.GetCurrentTile());
            }
        }
        
        Debug.Log("Map generation complete!");
    }
    
    private Vector3Int OffsetToAxial(int x, int y, GridLayout layout)
    {
        int q, r;
        
        switch (layout)
        {
            case GridLayout.OddROffset:
                // Odd-r offset: odd rows are shifted right
                // q = col - (row - (row&1)) / 2
                q = x - (y - (y & 1)) / 2;
                r = y;
                break;
                
            case GridLayout.EvenROffset:
                // Even-r offset: even rows are shifted right
                // q = col - (row + (row&1)) / 2
                q = x - (y + (y & 1)) / 2;
                r = y;
                break;
                
            case GridLayout.OddQOffset:
                // Odd-q offset: odd columns are shifted down
                // r = row - (col - (col&1)) / 2
                q = x;
                r = y - (x - (x & 1)) / 2;
                break;
                
            case GridLayout.EvenQOffset:
                // Even-q offset: even columns are shifted down
                // r = row - (col + (col&1)) / 2
                q = x;
                r = y - (x + (x & 1)) / 2;
                break;
                
            case GridLayout.Simple:
                // No offset - creates diamond/rhombus shape
                q = x;
                r = y;
                break;
                
            default:
                q = x;
                r = y;
                break;
        }
        
        return new Vector3Int(q, r, 0);
    }
    
    /// <summary>
    /// Clears all tiles from the map
    /// </summary>
    public void ClearMap()
    {
        // Get all tile positions and remove them
        var tilesToRemove = new System.Collections.Generic.List<Vector3Int>(TilemapManager.instance.tiles.Keys);
        foreach (var position in tilesToRemove)
        {
            TilemapManager.instance.RemoveTileAt(position);
        }
        
        Debug.Log("Map cleared!");
    }
}
