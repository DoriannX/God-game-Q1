using System.Collections;
using System.Linq;
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
    private Collider2D[] results = new Collider2D[1000];

    public override void Destroy(HeightManager heightManager, Vector2 pos)
    {
        StartCoroutine(DestroyAfterDelay(heightManager, pos));
    }

    private IEnumerator DestroyAfterDelay(HeightManager heightManager, Vector2 pos)
    {
        float waitTime = Random.Range(minDelay, maxDelay);
        Instantiate(meteoriteAnimationPrefab, transform.position, Quaternion.identity).GetComponent<MeteoriteAnimation>().SetDuration(waitTime);
        yield return new WaitForSeconds(waitTime);

        // Impact visuel
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
                    WaterSystem.instance.RemoveWater(cell);
                }
            }
        }

        // Destruction des objets touchés
        ContactFilter2D contactFilter = new()
        {
            useTriggers = true,
            useLayerMask = false,
            useDepth = false,
            useOutsideDepth = false,
            useNormalAngle = false,
            useOutsideNormalAngle = false
        };
        int hitCount = Physics2D.OverlapCircle(transform.position, meteoriteSize, contactFilter, results);
        print(hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            var destructible = results[i].GetComponent<PosableObject>();
            if (destructible != null)
            {
                if( destructible.GetComponent<GhostIa>() != null)
                {
                    GhostManager.instance.RemoveGhost(destructible.gameObject);
                }
                else if (destructible.GetComponent<House>() != null)
                {
                    House house = destructible.GetComponent<House>();
                    for (int numberOfGhostInHouse = 0; numberOfGhostInHouse < house.fuckingGhosts.Count; numberOfGhostInHouse++)
                    {
                        GhostManager.instance.UnregisterGhost(house.fuckingGhosts.ElementAt(numberOfGhostInHouse));
                    }
                    Destroy(destructible.gameObject);   
                }
                else
                {
                    Destroy(destructible.gameObject);
                }
            }
                
        }

        // Destruction du météore lui-même
        Destroy(gameObject);
    }
}
