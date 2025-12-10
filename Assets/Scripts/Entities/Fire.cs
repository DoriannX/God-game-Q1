using System.Collections;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float spreadRate;
    [SerializeField] private Vector3Int currentTile;

    private int currentTopZ;
    
    private static readonly Vector3Int[] hexNeighbors = new Vector3Int[]
    {
        new Vector3Int(+1, 0, 0),
        new Vector3Int(+1, -1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, +1, 0),
        new Vector3Int(0, +1, 0)
    };

    private void Start()
    {
        currentTile = TilemapManager.instance.WorldToHexAxial(transform.position);
        
        currentTopZ = TilemapManager.instance.GetColumnTopCoordinate(
            new Vector2Int(currentTile.x, currentTile.y)
        );
        
        FireManager.Instance.fireList.Add(this);
        
        StartCoroutine(SpreadFire());
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        
        FireManager.Instance.UnregisterFire(this);

        Destroy(gameObject);
    }

    private IEnumerator SpreadFire()
    {
        yield return new WaitForSeconds(spreadRate);

        foreach (var offset in hexNeighbors)
        {
            Vector3Int neighbor = currentTile + offset;
            
            int neighborTopZ = TilemapManager.instance.GetColumnTopCoordinate(
                new Vector2Int(neighbor.x, neighbor.y)
            );
            
            if (neighborTopZ != currentTopZ)
                continue;
            
            FireManager.Instance.TrySpawnFire(neighbor);

            yield return null;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        House houseHit = collision.gameObject.GetComponent<House>();
        Tree treeHit = collision.gameObject.GetComponent<Tree>();
        if (houseHit != null)
        {
            GhostManager.instance.UnregisterGhostInHouse(houseHit);
            Destroy(houseHit.gameObject);
        }
        else if (treeHit != null)
        {
            Destroy(treeHit.gameObject);
        }
    }
}
