using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages brush size for painting or modifying tiles in a hexagonal tilemap.
/// </summary>
public class BrushSizeManager : MonoBehaviour
{
    [Header("Brush Size Settings")]
    [SerializeField] private int minBrushSize = 1;
    [SerializeField] private int maxBrushSize = 10;
    [SerializeField] private int defaultBrushSize = 1;
    [SerializeField] private float scrollSensitivity = 1f;

    public int brushSize { get; private set; }

    public delegate void BrushSizeChangedDelegate(int newSize);
    public event BrushSizeChangedDelegate onBrushSizeChanged;
    
    public static BrushSizeManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        brushSize = defaultBrushSize;
    }
    
    /// <summary>
    ///  Changes the brush size based on scroll delta input
    /// </summary>
    public void ChangeBrushSize(float scrollDelta)
    {
        int oldSize = brushSize;
        
        //To avoid too small changes used Ceil and Floor
        if (scrollDelta > 0)
        {
            brushSize = Mathf.Min(brushSize + Mathf.CeilToInt(scrollDelta * scrollSensitivity), maxBrushSize);
        }
        else if (scrollDelta < 0)
        {
            brushSize = Mathf.Max(brushSize + Mathf.FloorToInt(scrollDelta * scrollSensitivity), minBrushSize);
        }
        
        if (oldSize != brushSize)
        {
            onBrushSizeChanged?.Invoke(brushSize);
        }
    }
    
    /// <summary>
    ///  Sets the brush size to a specific value
    /// </summary>
    public void SetBrushSize(int size)
    {
        int clampedSize = Mathf.Clamp(size, minBrushSize, maxBrushSize);
        
        if (clampedSize != brushSize)
        {
            brushSize = clampedSize;
            onBrushSizeChanged?.Invoke(brushSize);
        }
    }
    
    /// <summary>
    ///  Gets the hexagonal area covered by the brush based on its size and center position
    /// </summary>
    public Vector2Int[] GetBrushArea(Vector3Int centerHex)
    {
        if (brushSize == 1)
        {
            return new Vector2Int[] { new(centerHex.x, centerHex.y) };
        }
        
        int radius = brushSize - 1;
        List<Vector2Int> hexagons = new();
        
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            
            for (int r = r1; r <= r2; r++)
            {
                hexagons.Add(new Vector2Int(centerHex.x + q, centerHex.y + r));
            }
        }
        
        return hexagons.ToArray();
    }
}

