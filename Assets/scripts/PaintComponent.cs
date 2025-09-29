using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PaintComponent : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [field: SerializeField] public Tile[] tiles { get; private set; }
    public int tileIndex { get; private set; }
    private Vector2 lastPos;
    private Vector2 lastHitPos;

    public void Add(Vector2 pos, float brushSize)
    {
        foreach (Vector3Int cell in tilemap.cellBounds.allPositionsWithin)
        {
            Vector3 cellWorldPos = tilemap.GetCellCenterWorld(cell);

            if (Vector2.Distance(pos, cellWorldPos) <= brushSize)
            {
                tilemap.SetTile(cell, tiles[tileIndex]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastPos, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lastHitPos, 0.5f);
    }

    public void SetCurrentTile(int index)
    {
        tileIndex = index;
    }
}