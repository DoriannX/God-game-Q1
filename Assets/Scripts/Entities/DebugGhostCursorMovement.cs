/*
using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugGhostCursorMovement : MonoBehaviour
{
    private Camera camera1;
    private GhostMovement ghostMovement;
    private HexPathfinding2D pathFinding;

    private void Start()
    {
        ghostMovement = GetComponent<GhostMovement>();
        pathFinding = GetComponent<HexPathfinding2D>();
        camera1 = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        if (!camera1)
        {
            return;
        }

        Vector2 mousePos = camera1.ScreenToWorldPoint(Input.mousePosition);
        List<Vector3Int> positions = pathFinding.FindPath(transform.position, mousePos);
        List<Vector2> worldPositions = pathFinding.GetWorldPath(positions);
        ghostMovement.GoTo(worldPositions);
    }
}
*/
