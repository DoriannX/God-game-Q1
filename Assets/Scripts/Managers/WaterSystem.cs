using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// TEMPORARILY DISABLED - Water system incompatible with new hexagonal TilemapManager.
/// TODO: Re-implement water system for hexagonal grid once tile placement is stable.
/// </summary>
public class WaterSystem : MonoBehaviour
{
    public static WaterSystem instance { get; private set; }
    
    [Header("SYSTEM DISABLED - See class documentation")]
    [SerializeField] private PaintComponent paintComponent;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private HeightManager heightManager;

    // All water logic disabled - keeping fields to prevent serialization errors
    private Queue<Vector3Int> propagationQueue = new();
    private HashSet<Vector3Int> activeTiles = new();
    public HashSet<Vector3Int> waterTiles { get; } = new();
    private readonly List<Vector3Int> pendingRemovals = new();

    private static readonly Vector3Int[] evenNeighborOffsets =
    {
        new(1, 0, 0), new(-1, 0, 0), new(0, 1, 0), new(-1, 1, 0), new(0, -1, 0), new(-1, -1, 0)
    };

    private static readonly Vector3Int[] oddNeighborOffsets =
    {
        new(1, 0, 0), new(-1, 0, 0), new(1, 1, 0), new(0, 1, 0), new(1, -1, 0), new(0, -1, 0)
    };

    private void Awake()
    {
        // DISABLED: Water system temporarily disabled for new hexagonal TilemapManager
        // paintComponent.paintedWater += DrawWater;
        // TickSystem.ticked += Tick;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        
        Debug.LogWarning("WaterSystem is DISABLED - Incompatible with new hexagonal TilemapManager");
    }

    private void Start()
    {
        // DISABLED: All water initialization commented out
    }

    private void Tick()
    {
        // DISABLED
    }

    public bool IsOnWater(Vector2 position)
    {
        // DISABLED: Always return false
        return false;
    }

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        // DISABLED: Return empty enumerable
        yield break;
    }

    private void Expand()
    {
        // DISABLED
    }

    private bool AnyNeighborHasWater(Vector3Int pos)
    {
        // DISABLED
        return false;
    }

    public void AddWater(Vector3Int position)
    {
        // DISABLED
    }

    public void DrawWater(Vector3Int position)
    {
        // DISABLED
    }

    public void ActivateOneAdjacent(Vector3Int position)
    {
        // DISABLED
    }

    public void RemoveWater(Vector3Int position)
    {
        // DISABLED
    }
    
    private void ProcessPendingRemovals()
    {
        // DISABLED
    }

    public void ClearAllWater()
    {
        // DISABLED
    }
}