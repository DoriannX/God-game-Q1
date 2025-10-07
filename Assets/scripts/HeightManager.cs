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
}
