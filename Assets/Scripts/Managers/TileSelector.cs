using System;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    [Serializable]
    public class TileData
    {
        public string tileName;
        public Sprite tileIcon;
        public GameObject tilePrefab;
    }

    [field: Header("Tile Selection")]
    [field: SerializeField]
    public TileData[] AvailableTiles { get; private set; }

    public int currentTileIndex { get; private set; } = 0;
    
    public static TileSelector instance { get; private set; }

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
        if (AvailableTiles == null || AvailableTiles.Length == 0)
        {
            Debug.LogWarning("No tiles available in TileSelector!");
        }
        else
        {
            currentTileIndex = Mathf.Clamp(currentTileIndex, 0, AvailableTiles.Length - 1);
        }
    }
    
    /// <summary>
    ///  Returns the currently selected tile data.
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentTile()
    {
        return  AvailableTiles is { Length: > 0 }
            ? AvailableTiles[currentTileIndex].tilePrefab
            : null;
    }
    
    /// <summary>
    ///  Sets the current tile index to the specified index.
    /// </summary>
    /// <param name="index"></param>
    public void SetCurrentTileIndex(int index)
    {
        if (AvailableTiles != null && AvailableTiles.Length > 0)
        {
            currentTileIndex = Mathf.Clamp(index, 0, AvailableTiles.Length - 1);
        }
    }
}

