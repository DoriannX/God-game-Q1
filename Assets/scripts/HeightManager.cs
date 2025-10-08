using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class HeightManager : MonoBehaviour
{
    [SerializeField] private List<HeightCell> heightCells;
    
    public TileBase GetUnderHeightTile(TileBase tile)
    {
        HeightCell cell = heightCells.Find(hc => hc.tile == tile);
        return cell != null ? cell.previousHeightTile : null;
    }
    
    public TileBase GetUnderHeightTile(TileBase tile, int levels)
    {
        TileBase underTile = tile;
        for(int i = 0; i < levels; i++)
        {
            TileBase nextTile = GetUnderHeightTile(underTile);
            if (nextTile == null) break;
            underTile = nextTile;
        }
        return underTile;
    }
    
    public int GetHeightIndex(TileBase tile)
    {
        TileBase underTile = GetUnderHeightTile(tile);
        int index = 0;
        while(underTile != null)
        {
            index++;
            tile = underTile;
            underTile = GetUnderHeightTile(tile);
        }
        return index;
    }
    
    public int GetTotalHeightLevels()
    {
        int maxIndex = 0;
        foreach(var cell in heightCells)
        {
            int index = GetHeightIndex(cell.tile);
            if (index > maxIndex)
                maxIndex = index;
        }
        return maxIndex + 1; // +1 to count the base level
    }
}
