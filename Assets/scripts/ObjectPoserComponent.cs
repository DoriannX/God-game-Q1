using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ObjectPoserComponent : MonoBehaviour
{
    [field: SerializeField] public PosableObject[] posableObjects { get; private set; }
    public int objectIndex { get; private set; }
    private Vector2 lastPos;
    private Vector2 lastHitPos;
    private bool canPlace = true;
    private Camera camera1;

    private void Start()
    {
        camera1 = Camera.main;
    }

    private void OnEnable()
    {
        TickSystem.ticked += ResetAdd;
    }

    public void Add(Vector2 pos, float brushSize)
    {
        Add(pos, brushSize, posableObjects[objectIndex]);
    }

    private void Add(Vector2 pos, float brushSize, PosableObject posableObject)
    {
        if (!canPlace) return;

        var tilemap = TilemapManager.instance;
        var worldPos = new Vector3(pos.x, pos.y, 0);
        var allowedTiles = posableObject.allowedTiles;

        if (Mathf.Approximately(brushSize, 0.1f))
        {
            TryPlace(tilemap, worldPos, allowedTiles, posableObject);
            return;
        }

        int randomCount = Random.Range(1, (int)(brushSize * 10));
        for (int i = 0; i < randomCount; i++)
        {
            var offset = new Vector3(Random.Range(-brushSize, brushSize), Random.Range(-brushSize, brushSize), 0);
            TryPlace(tilemap, worldPos + offset, allowedTiles, posableObject);
        }
    }

    private void TryPlace(TilemapManager tilemap, Vector3 worldPos, List<TileBase> allowedTiles,
        PosableObject prefab)
    {
        var cellPos = tilemap.WorldToCell(worldPos);
        var baseTile = tilemap.GetTile(cellPos);
        if (baseTile == null || !allowedTiles.Contains(baseTile)) return;

        var waterTile = tilemap.GetWaterTile(cellPos);
        if (waterTile != null && !allowedTiles.Contains(waterTile)) return;

        Instantiate(prefab, tilemap.GetCellCenterWorld(cellPos), Quaternion.identity);
        canPlace = false;
    }

    public void Remove(Vector2 pos, float brushSize)
    {
        lastPos = pos;
        var worldPos = new Vector3(pos.x, pos.y, 0);

        if (Mathf.Approximately(brushSize, 0.1f))
        {
            var ray = camera1.ScreenPointToRay(camera1.WorldToScreenPoint(worldPos));
            var hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider == null) return;

            var obj = hit.collider.GetComponent<PosableObject>();
            if (obj == null) return;

            Destroy(hit.collider.gameObject);
            print("Destroyed object at " + hit.point);
            lastHitPos = hit.point;
            return;
        }

        var colliders = Physics2D.OverlapCircleAll(worldPos, brushSize);
        foreach (var col in colliders)
        {
            var obj = col.GetComponent<PosableObject>();
            if (obj != null) Destroy(col.gameObject);
        }
    }

    private void ResetAdd()
    {
        canPlace = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastPos, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(lastHitPos, 0.5f);
    }

    private void OnDisable()
    {
        TickSystem.ticked -= ResetAdd;
    }

    public void SetCurrentObject(int index)
    {
        objectIndex = index;
    }
}