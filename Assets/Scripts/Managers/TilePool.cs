using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

/// <summary>
/// A pool manager for tile GameObjects
/// </summary>
public class TilePool : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int defaultCapacity = 50;
    [SerializeField] private int maxSize = 500;
    [SerializeField] private bool prewarmOnStart = false;
    
    private ObjectPool<GameObject> pool;
    private bool isQuitting = false;
    
    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: CreateTile,
            actionOnGet: OnGetTile,
            actionOnRelease: OnReleaseTile,
            actionOnDestroy: OnDestroyTile,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        
        if (prewarmOnStart)
        {
            StartCoroutine(PrewarmPoolGradually());
        }
    }
    
    /// <summary>
    /// Instantiate tiles gradually to avoid frame drops
    /// </summary>
    private IEnumerator PrewarmPoolGradually()
    {
        GameObject[] prewarmTiles = new GameObject[defaultCapacity];
        
        int batchSize = 10;
        for (int i = 0; i < defaultCapacity; i++)
        {
            prewarmTiles[i] = pool.Get();
            
            if ((i + 1) % batchSize == 0)
            {
                yield return null;
            }
        }
        
        for (int i = 0; i < defaultCapacity; i++) // Release all prewarmed tiles back to the pool because Get() activates them
        {
            pool.Release(prewarmTiles[i]);
        }
    }
    
    private GameObject CreateTile()
    {
        GameObject tile = Instantiate(tilePrefab);
        tile.transform.SetParent(transform);
        return tile;
    }
    
    private void OnGetTile(GameObject tile)
    {
        tile.SetActive(true);
    }
    
    private void OnReleaseTile(GameObject tile)
    {
        tile.SetActive(false);
    }
    
    private void OnDestroyTile(GameObject tile)
    {
        if (tile == null || isQuitting) return;
        
        if (Application.isPlaying)
        {
            Destroy(tile);
        }
        else
        {
            DestroyImmediate(tile); // In case the tile pool is killed just after exiting play mode in the editor
        }
    }
    
    private void OnApplicationQuit()
    {
        isQuitting = true; // Prevents trying to destroy tiles during application quit
    }
    
    private void OnDestroy()
    {
        isQuitting = true;
    }
    
    public GameObject GetTile()
    {
        if (isQuitting) return null;
        return pool.Get();
    }
    
    public void ReleaseTile(GameObject tile)
    {
        if (isQuitting || tile == null) return;
        pool.Release(tile);
    }
}

