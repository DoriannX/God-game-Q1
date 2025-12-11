using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteore : DestructionObject
{
    [SerializeField] private float minSize = 3f;
    [SerializeField] private float maxSize = 7f;
    [SerializeField] private float minDelay = 1f;
    [SerializeField] private float maxDelay = 3f;
    [SerializeField] private GameObject meteoriteAnimationPrefab;

    private void OnEnable()
    {
        Destroy();
    }

    public override void Destroy()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    /// <summary>
    ///  Destroy the meteorite after a random delay, creating a spherical impact on tiles and objects
    /// </summary>
    private IEnumerator DestroyAfterDelay()
    {
        print("éclate dans " + gameObject.name);
        float waitTime = Random.Range(minDelay, maxDelay);
        Instantiate(meteoriteAnimationPrefab, transform.position, Quaternion.identity).GetComponent<MeteoriteAnimation>().SetDuration(waitTime);
        yield return new WaitForSeconds(waitTime);

        // Simple spherical impact - use OverlapSphere to detect everything
        Vector3 impactCenter = transform.position;
        float meteoriteSize = Random.Range(minSize, maxSize);

        // Get all colliders in the impact sphere
        Collider[] hitColliders = Physics.OverlapSphere(impactCenter, meteoriteSize);
        
        foreach (Collider hitCol in hitColliders)
        {
            // Check if it's a tile
            GameObject hitObject = hitCol.gameObject;
            Vector3Int tileCoords = TilemapManager.instance.WorldToHexAxial(hitObject.transform.position);
            
            // Try to get the tile at this position
            GameObject tile = TilemapManager.instance.GetTile(tileCoords);
            if (tile == hitObject)
            {
                // It's a tile - remove it from the tilemap
                TilemapManager.instance.RemoveTileAt(tileCoords);
                
                // Also check for placed objects one level above
                Vector3Int objectPos = tileCoords + new Vector3Int(0, 0, 1);
                var placedObject = TilemapManager.instance.GetPlacedObjectAt(objectPos);
                if (placedObject != null)
                {
                    HandlePosableDestruction(placedObject);
                    TilemapManager.instance.RemovePlacedObjectAt(objectPos);
                }
            }
            else
            {
                // It's an entity or placed object - handle destruction
                var posable = hitObject.GetComponent<Posable>();
                if (posable != null)
                {
                    HandlePosableDestruction(posable);
                }
            }
        }

        // Destruction du météore lui-même
        Destroy(gameObject);
        yield return null;
    }

    /// <summary>
    /// Handles the destruction of posable objects (ghosts, houses, etc.)
    /// </summary>
    private void HandlePosableDestruction(Posable posable)
    {
        if (posable.GetComponent<GhostIa>() != null)
        {
            GhostManager.instance.RemoveGhost(posable.gameObject);
        }
        else if (posable.GetComponent<House>() != null)
        {
            GhostManager.instance.UnregisterGhostInHouse(posable.GetComponent<House>());
            Destroy(posable.gameObject);
        }
        else
        {
            Destroy(posable.gameObject);
        }
    }
}

