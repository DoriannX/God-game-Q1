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
        targetPosition = position;
        ComputePath();
    }
    
    public void GoBy(Vector2 direction)
    {
        targetPosition = (Vector2)transform.position + direction.normalized * TilemapManager.instance.cellSize;
        ComputePath();
    }

    private void Tick()
    {
        growComponent.Grow();
        if (ghostMovement.isMoving)
            return;
        ComputePath();

        if ( currentPath == null || currentPath.Count < 2)
        {
            return;
        }

        ghostMovement.GoTo(currentPath[1]);
    }

    public bool isMoving =>
        Vector2.Distance(transform.position, targetPosition) > 0.01f && currentPath != null && currentPath.Count > 1;

    public void ForceRepath() => ComputePath();

    private void ComputePath()
    {
        Vector3Int startCell = TilemapManager.instance.WorldToCell(transform.position);
        Vector3Int goalCell = TilemapManager.instance.WorldToCell(targetPosition);

        List<Vector3Int> tilePath = HexPathfinding2D.instance.FindPath(startCell, goalCell);
        currentPath = HexPathfinding2D.instance.GetWorldPath(tilePath);
    }
    
    public Vector2? GetRandomWalkablePosition(float radius)
    {
        Vector3Int? cell = HexPathfinding2D.instance.GetRandomWalkableCell(transform.position, radius);
        if (cell == null) return null;
        return TilemapManager.instance.GetCellCenterWorld(cell.Value);
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
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }
}