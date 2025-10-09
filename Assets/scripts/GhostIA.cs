using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostIA : MonoBehaviour
{
    [SerializeField] private GhostMovement ghostMovement;
    [SerializeField] private HexPathfinding2D pathFinding;

    private Vector2 targetPosition;
    private List<Vector2> currentPath;
    private Vector3Int lastStart;
    private Vector3Int lastGoal;

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

    private void Tick()
    {
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
        Vector3Int startCell = TilemapManager.instance.tilemap.WorldToCell(transform.position);
        Vector3Int goalCell = TilemapManager.instance.tilemap.WorldToCell(targetPosition);

        List<Vector3Int> tilePath = pathFinding.FindPath(startCell, goalCell);
        currentPath = pathFinding.GetWorldPath(tilePath);

        lastStart = startCell;
        lastGoal = goalCell;
    }
    
    public Vector2? GetRandomWalkablePosition(float radius)
    {
        var cell = pathFinding.GetRandomWalkableCell(radius);
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
}