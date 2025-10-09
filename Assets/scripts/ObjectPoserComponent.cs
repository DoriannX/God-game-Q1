using System;
using CTools.CTimer;
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
        if(!canPlace) return;
        print(brushSize * 10 + " objects to place");
        int randomCount = Random.Range(1, (int)(brushSize * 10));
        for (var i = 0; i < randomCount; i++)
        {
            Vector3 worldPos = new(pos.x, pos.y, 0);
            var offset = new Vector3(Random.Range(-brushSize, brushSize), Random.Range(-brushSize, brushSize), 0);
            Vector3Int cellPos = TilemapManager.instance.WorldToCell(worldPos + offset);
            if (TilemapManager.instance.GetTile(cellPos) == null ||
                !posableObject.allowedTiles.Contains(TilemapManager.instance.GetTile(cellPos)) ||
                (TilemapManager.instance.GetWaterTile(cellPos) != null && !posableObject.allowedTiles.Contains(TilemapManager.instance.GetWaterTile(cellPos))))
            {
                continue;
            }
            Vector3 spawnPos = worldPos + offset;
            Instantiate(posableObject, spawnPos, Quaternion.identity);
            canPlace = false;
        }
    }
    
    public void Remove(Vector2 pos, float brushSize)
    {
        print(brushSize * 10 + " objects to remove");
        Vector3 worldPos = new(pos.x, pos.y, 0);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, brushSize);
        foreach (var collider in colliders)
        {
            PosableObject posableObject = collider.GetComponent<PosableObject>();
            if (posableObject != null)
            {
                Destroy(collider.gameObject);
            }
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