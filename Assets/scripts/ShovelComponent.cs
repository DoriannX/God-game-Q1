using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShovelComponent : MonoBehaviour
{
    private HeightManager heightManager;
    private List<Vector3Int> modifiedCells = new();

    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }

    private void Start()
    {
        heightManager = HeightManager.instance;
    }

    public void Add(Vector2 pos, float brushSize)
    {
        Vector3Int centerCell = TilemapManager.instance.WorldToCell(pos);

        int cellRadiusX = Mathf.CeilToInt(brushSize * 1.5f / TilemapManager.instance.cellSize.x);
        int cellRadiusY = Mathf.CeilToInt(brushSize * 1.5f / TilemapManager.instance.cellSize.y);

        int maxRadius = Mathf.Max(cellRadiusX, cellRadiusY);

        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = TilemapManager.instance.GetCellCenterWorld(cell);

                if (Vector2.Distance(pos, cellWorldPos) <= brushSize )
                {
                    if(!modifiedCells.Contains(cell))
                    {
                        modifiedCells.Add(cell);
                        TileBase previousTile = heightManager.GetUnderHeightTile(TilemapManager.instance.GetTile(cell));
                        if (previousTile == null) continue;
                        TilemapManager.instance.SetTile(cell, previousTile);
                    }
                    WaterSystem.instance?.RemoveWater(cell);
                }
            }
        }
    }

    private void OnTick()
    {
        for (int i = 0; i < modifiedCells.ToList().Count; i++)
        {
            modifiedCells.Remove(modifiedCells[i]);
        }
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}