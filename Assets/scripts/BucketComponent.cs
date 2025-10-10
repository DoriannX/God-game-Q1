using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BucketComponent : MonoBehaviour
{

    public void Remove(Vector2 pos, float brushSize)
    {
        Vector3Int centerCell = TilemapManager.instance.WorldToWaterCell(pos);

        int cellRadiusX = Mathf.CeilToInt(brushSize * 1.5f / TilemapManager.instance.cellSize.x);
        int cellRadiusY = Mathf.CeilToInt(brushSize * 1.5f / TilemapManager.instance.cellSize.y);

        int maxRadius = Mathf.Max(cellRadiusX, cellRadiusY);

        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = TilemapManager.instance.GetWaterCellCenterWorld(cell);

                if (Vector2.Distance(pos, cellWorldPos) <= brushSize)
                {
                    WaterSystem.instance.RemoveWaterTile(cell);
                    WaterSystem.instance.ReactivateAdjacentWater(cell);
                }
            }
        }
    }
}