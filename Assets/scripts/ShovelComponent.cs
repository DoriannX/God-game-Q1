using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShovelComponent : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private HeightManager heightManager;
    private List<Vector3Int> modifiedCells = new();
    private List<float> modificationTimes = new();
    [SerializeField] private float shovelDelay = 1f;

    public void Add(Vector2 pos, float brushSize)
    {
        Vector3Int centerCell = tilemap.WorldToCell(pos);

        int cellRadiusX = Mathf.CeilToInt(brushSize * 1.5f / tilemap.cellSize.x);
        int cellRadiusY = Mathf.CeilToInt(brushSize * 1.5f / tilemap.cellSize.y);

        int maxRadius = Mathf.Max(cellRadiusX, cellRadiusY);

        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = tilemap.GetCellCenterWorld(cell);

                if (Vector2.Distance(pos, cellWorldPos) <= brushSize && !modifiedCells.Contains(cell))
                {
                    modifiedCells.Add(cell);
                    modificationTimes.Add(Time.time);
                    TileBase previousTile = heightManager.GetUnderHeightTile(tilemap.GetTile(cell));
                    if (previousTile == null) continue;
                    tilemap.SetTile(cell, previousTile);
                }
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < modifiedCells.ToList().Count; i++)
        {
            if (modificationTimes[i] + shovelDelay < Time.time)
            {
                modifiedCells.Remove(modifiedCells[i]);
                modificationTimes.Remove(modificationTimes[i]);
            }
        }
    }
}