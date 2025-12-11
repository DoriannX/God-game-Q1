using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A pool manager for tile GameObjects supporting multiple prefab types
/// </summary>
public class TilePool : MonoBehaviour
{
    [SerializeField] private int defaultCapacity = 50;
    [SerializeField] private int maxSize = 500;
    [SerializeField] private bool prewarmOnStart = false;
    
    private Dictionary<GameObject, ObjectPool<GameObject>> pools = new();
    private Dictionary<GameObject, GameObject> tileToOriginalPrefab = new();
    private bool isQuitting = false;
    
    private void Awake()
    {
        if (prewarmOnStart)
        {
            StartCoroutine(PrewarmPoolsGradually());
        }
    }
    
    /// <summary>
    /// Instantiate tiles gradually to avoid frame drops
    /// </summary>
    private IEnumerator PrewarmPoolsGradually()
    {
        foreach (var poolPair in pools)
        {
            GameObject[] prewarmTiles = new GameObject[defaultCapacity];
            
            int batchSize = 10;
            for (int i = 0; i < defaultCapacity; i++)
            {
                prewarmTiles[i] = poolPair.Value.Get();
                
                if ((i + 1) % batchSize == 0)
                {
                    yield return null;
                }
            }
            
            for (int i = 0; i < defaultCapacity; i++)
            {
                poolPair.Value.Release(prewarmTiles[i]);
            }
        }
    }
    
    private GameObject CreateTile(GameObject prefab)
    {
        GameObject tile = Instantiate(prefab);
        tile.transform.SetParent(transform);
        tileToOriginalPrefab[tile] = prefab;
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
        
        tileToOriginalPrefab.Remove(tile);
        
        if (Application.isPlaying)
        {
            Destroy(tile);
        }
        else
        {
            DestroyImmediate(tile);
        }
    }
    
    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
    
    private void OnDestroy()
    {
        isQuitting = true;
    }
    
    private ObjectPool<GameObject> GetOrCreatePool(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new ObjectPool<GameObject>(
                createFunc: () => CreateTile(prefab),
                actionOnGet: OnGetTile,
                actionOnRelease: OnReleaseTile,
                actionOnDestroy: OnDestroyTile,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
        return pools[prefab];
    }
    
    public GameObject GetTile(GameObject prefab)
    {
        if (isQuitting || prefab == null) return null;
        
        ObjectPool<GameObject> pool = GetOrCreatePool(prefab);
        return pool.Get();
    }
    
    public void ReleaseTile(GameObject tile)
    {
        if (isQuitting || tile == null) return;
        
        if (tileToOriginalPrefab.TryGetValue(tile, out GameObject prefab))
        {
            if (pools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(tile);
            }
        }
    }
    
    public GameObject GetOriginalPrefab(GameObject tile)
    {
        if (tile == null) return null;
        
        tileToOriginalPrefab.TryGetValue(tile, out GameObject prefab);
        return prefab;
    }
}

