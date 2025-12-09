using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;

public class GhostIa : MonoBehaviour, ISaveable 
{
    [Serializable]
    public struct GhostData
    {
        public SaveableEntity.Vector2Data position;
        public SaveableEntity.Vector2Data targetPosition;
        public List<SaveableEntity.Vector2Data> currentPath;
    }
    [SerializeField] private GhostMovement ghostMovement;
    [SerializeField] private GrowComponent growComponent;

    public Vector2 targetPosition { get; private set; }
    private List<Vector2> currentPath;
    private bool usePathFinding = false;
    private int currentHeight;
    private void OnEnable()
    {
        TickSystem.ticked += Tick;
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= Tick;
    }

    public void GoTo(Vector2 position)
    {
        usePathFinding = true;
        targetPosition = position;
        ComputePath();
    }
    
    public void GoBy(Vector2 direction)
    {
        //TODO : refaire
        /*targetPosition = (Vector2)transform.position + direction.normalized * TilemapManager.instance.cellSize;
        usePathFinding = true;
        ComputePath();*/
    }
    
    public void GoByRandom()
    {
        //TODO : refiare
        /*const int maxAttempts = 12;
        Vector2 origin = transform.position;
        
        // Cache frequently accessed instances
        var tilemapManager = TilemapManager.instance;
        var waterSystem = WaterSystem.instance;
        var heightManager = HeightManager.instance;
        Vector3 cellSize = tilemapManager.cellSize;
        
        // Get current height once instead of in every iteration
        Vector3Int currentCell = tilemapManager.WorldToHexAxial(origin);
        currentHeight = heightManager.GetHeightIndex(tilemapManager.GetTile(currentCell));
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 candidate = origin + dir * cellSize;
                
            Vector3Int candidateCell = tilemapManager.WorldToHexAxial(candidate);
            if(waterSystem.waterTiles.Contains(candidateCell))
                continue;
            int candidateHeight = heightManager.GetHeightIndex(tilemapManager.GetTile(candidateCell));
            
            if (candidateHeight - 1 <= currentHeight)
            {
                targetPosition = TilemapManager.instance.HexAxialToWorld(candidateCell);
                usePathFinding = false;
                currentPath = new List<Vector2> { origin, targetPosition };
                return;
            }
        }
        
        // No valid position found
        targetPosition = origin;
        currentPath = null;*/
    }

    private void Tick()
    {
        growComponent.Grow();
        if (ghostMovement.isMoving)
            return;
        if (usePathFinding)
            ComputePath();

        if ( currentPath == null || currentPath.Count < 2)
        {
            return;
        }
        ghostMovement.GoTo(currentPath[1], currentHeight);
    }

    public bool isMoving =>
        Vector2.Distance(transform.position, targetPosition) > 0.01f && currentPath != null && currentPath.Count > 1;

    public void ForceRepath() => ComputePath();

    private void ComputePath()
    {
        currentPath = HexPathfinding2D.instance.FindPath(transform.position, targetPosition);
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null || currentPath.Count == 0) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
        }
    }

    public bool NeedsToBeSaved()
    {
        return true;
    }

    public bool NeedsReinstantiation()
    {
        return true;
    }

    public object SaveState()
    {
        var data = new GhostData
        {
            position = new SaveableEntity.Vector2Data(transform.position),
            targetPosition = new SaveableEntity.Vector2Data(targetPosition),
            currentPath = new List<SaveableEntity.Vector2Data>()
        };
        if (currentPath != null)
        {
            foreach (var pos in currentPath)
            {
                data.currentPath.Add(new SaveableEntity.Vector2Data(pos));
            }
        }
        return data;
    }

    public void LoadState(object state)
    {
        var data = (GhostData)state;
        transform.position = data.position.ToVector2();
        targetPosition = data.targetPosition.ToVector2();
        currentPath = new List<Vector2>();
        if (data.currentPath != null)
        {
            foreach (var pos in data.currentPath)
            {
                currentPath.Add(pos.ToVector2());
            }
        }
        else
        {
            currentPath = null;
        }
        GhostManager.instance.RegisterGhost(this);
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }
}