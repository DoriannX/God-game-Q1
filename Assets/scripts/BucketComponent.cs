using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BucketComponent : MonoBehaviour
{
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private WaterSystem waterSystem;

    public void Remove(Vector2 pos, float brushSize)
    {
        Vector3Int centerCell = waterTilemap.WorldToCell(pos);

        int cellRadiusX = Mathf.CeilToInt(brushSize * 1.5f / waterTilemap.cellSize.x);
        int cellRadiusY = Mathf.CeilToInt(brushSize * 1.5f / waterTilemap.cellSize.y);

        int maxRadius = Mathf.Max(cellRadiusX, cellRadiusY);

        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = waterTilemap.GetCellCenterWorld(cell);

                if (Vector2.Distance(pos, cellWorldPos) <= brushSize)
                {
                    waterSystem.RemoveWaterTile(cell);
                    waterSystem.ReactivateAdjacentWater(cell);
                }
            }
        }
    }
}