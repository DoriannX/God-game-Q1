using UnityEngine;

/// <summary>
/// TEMPORARILY DISABLED - Water removal component incompatible with new hexagonal TilemapManager.
/// TODO: Re-implement for hexagonal water system once water is re-enabled.
/// </summary>
public class BucketComponent : MonoBehaviour
{
    public void Remove(Vector2 pos, float brushSize)
    {
        // DISABLED: Water removal temporarily disabled
        Debug.LogWarning("BucketComponent.Remove() is DISABLED - Water system not available");
        
        /* ORIGINAL CODE - Disabled until water system is re-implemented
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
                    WaterSystem.instance.RemoveWater(cell);
                }
            }
        }
        */
    }
}