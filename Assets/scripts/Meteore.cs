using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Meteore : DestructionObject
{
    [SerializeField] private float minSize = 3f;
    [SerializeField] private float maxSize = 7f;
    [SerializeField] private float minDelay = 1f;
    [SerializeField] private float maxDelay = 3f;
    [SerializeField] private GameObject meteoriteAnimationPrefab;

    public override void Destroy(WaterSystem waterSystem, HeightManager heightManager, Vector2 pos)
    {
        StartCoroutine(DestroyAfterDelay(waterSystem, heightManager, pos));
    }

    private IEnumerator DestroyAfterDelay(WaterSystem waterSystem, HeightManager heightManager, Vector2 pos)
    {
        // Attente avant impact
        float waitTime = Random.Range(minDelay, maxDelay);
        Instantiate(meteoriteAnimationPrefab, transform.position, Quaternion.identity).GetComponent<MeteoriteAnimation>().SetDuration(waitTime);
        yield return new WaitForSeconds(waitTime);

        // Impact visuel
        Vector3 worldPos = new(pos.x, pos.y, 0);
        Vector3Int centerCell = TilemapManager.instance.WorldToCell(pos);
        float meteoriteSize = Random.Range(minSize, maxSize);

        int cellRadiusX = Mathf.CeilToInt(meteoriteSize * 1.5f / TilemapManager.instance.cellSize.x);
        int cellRadiusY = Mathf.CeilToInt(meteoriteSize * 1.5f / TilemapManager.instance.cellSize.y);
        int maxRadius = Mathf.Max(cellRadiusX, cellRadiusY);

        for (int x = -maxRadius; x <= maxRadius; x++)
        {
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = TilemapManager.instance.GetCellCenterWorld(cell);

                if (Vector2.Distance(pos, cellWorldPos) <= meteoriteSize)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), Vector2.zero);
                    float level = Mathf.Clamp01(1f - dist / maxRadius);
                    TileBase previousTile = heightManager.GetUnderHeightTile(
                        TilemapManager.instance.GetTile(cell),
                        (int)(level * 3)
                    );

                    if (previousTile == null) continue;
                    TilemapManager.instance.SetTile(cell, previousTile);
                    waterSystem.RemoveWaterTile(cell);
                    waterSystem.ReactivateAdjacentWater(cell);
                }
            }
        }

        // Destruction des objets touchés
        Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPos, meteoriteSize);
        foreach (Collider2D collider in colliders)
        {
            var posableObject = collider.GetComponent<PosableObject>();
            if (posableObject != null)
                Destroy(collider.gameObject);
        }

        // Destruction du météore lui-même
        Destroy(gameObject);
    }
}
