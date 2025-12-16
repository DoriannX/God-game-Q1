using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;
using Random = UnityEngine.Random;
using Components;

public abstract class EntityIA : MonoBehaviour, ISaveable {
    public virtual EntityType entityType { get; protected set; } = EntityType.Ghost;
    
    public Vector3 targetPosition { get; protected set; }
    
    [SerializeField] private EntityMovement entityMovement;
    
    protected List<Vector3> currentPath;
    private bool usePathFinding;
    private int currentHeight;
    
    TilemapManager tilemapManager;
    protected EntityManager entityManager;

    private void Start() {
        tilemapManager = TilemapManager.instance;
        entityManager = EntityManager.instance;
    }

    public void GoByRandom() {
        const int maxAttempts = 12;
        Vector3 origin = transform.position;
        // Cache frequently accessed instances
        Vector3 cellSize = tilemapManager.GetHexCellSize();
        
        Vector3Int currentCell = tilemapManager.WorldToHexAxial(origin);

        if(currentCell.z > 1) {
            currentCell.z --;
        }
        
        if(tilemapManager.GetTile(currentCell) == null) {
            Debug.Log("No tile found at " + currentCell);
            return;
        }
        
        for (int i = 0; i < maxAttempts; i++) {
            Vector3 randomVector= Random.insideUnitCircle.normalized;
            Vector3 dir = new(randomVector.x, 0, randomVector.y);
            Vector3 candidate = origin + Vector3.Scale(dir, cellSize);
            
            Vector3Int candidateCell = tilemapManager.WorldToHexAxial(candidate);
            GameObject candidateTile = tilemapManager.GetTile(candidateCell);
            if (candidateTile != null && candidateTile.GetComponent<WaterComponent>() != null)
            {
                continue;
            }
            
            if(candidateCell.z > 1) {
                candidateCell.z --;
            }

            if (tilemapManager.GetTile(candidateCell) == null || candidateCell == currentCell) 
            {
                continue;
            }

            if (tilemapManager.GetTile(candidateCell + Vector3Int.forward) != null) 
            {
                if (tilemapManager.GetTile(candidateCell + Vector3Int.forward * 2) == null) 
                {
                    candidateCell += Vector3Int.forward;
                }
                else 
                {
                    continue;
                }
            }
            
            targetPosition = TilemapManager.instance.HexAxialToWorld(candidateCell);
            usePathFinding = false;
            currentPath = new List<Vector3> { origin, targetPosition };
            return;
        }
        
        // No valid position found
        targetPosition = origin;
        currentPath = null;
    }
    public void GoTo(Vector3 position) {
        usePathFinding = true;
        targetPosition = position;
        ComputePath();
    }
    
    public void GoBy(Vector3 direction) {

        /*targetPosition = (Vector2)transform.position + direction.normalized * TilemapManager.instance.cellSize;
        targetPosition = transform.position +
                         Vector3.Scale(direction.normalized, TilemapManager.instance.GetHexCellSize());

        usePathFinding = true;
        ComputePath();*/
    }
    
    private void OnEnable() {
        TickSystem.ticked += Tick;
    }
    
    private void OnDisable() {
        TickSystem.ticked -= Tick;
    }
    
    private void Tick() {
        if (entityMovement.isMoving) {
            return;
        }

        if (usePathFinding) {
            ComputePath();
        }

        if ( currentPath == null || currentPath.Count < 2) {
            return;
        }
        
        entityMovement.GoTo(currentPath[1]);
    }
    
    public bool isMoving =>
        Vector2.Distance(transform.position, targetPosition) > 0.01f && currentPath != null && currentPath.Count > 1;

    protected void ComputePath() {
        Vector3Int currentCell = TilemapManager.instance.WorldToHexAxial(transform.position);
        Vector3Int targetCell = TilemapManager.instance.WorldToHexAxial(targetPosition);
        
        currentPath = HexPathfinding3D.instance.FindPath(transform.position, targetPosition, currentCell.z, targetCell.z);
    }
    
    public bool NeedsToBeSaved() {
        return true;
    }

    public bool NeedsReinstantiation() {
        return true;
    }

    public abstract object SaveState();

    public abstract void LoadState(object state);

    public virtual void PostInstantiation(object state){}

    public virtual void GotAddedAsChild(GameObject obj, GameObject hisParent){}
}
