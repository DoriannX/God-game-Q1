using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

public class TilePool : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int defaultCapacity = 50;
    [SerializeField] private int maxSize = 500;
    [SerializeField] private bool prewarmOnStart = false; // Désactiver le prewarming par défaut pour tester
    
    private ObjectPool<GameObject> pool;
    
    private void Awake()
    {
        // Créer le pool avec Unity's ObjectPool
        pool = new ObjectPool<GameObject>(
            createFunc: CreateTile,
            actionOnGet: OnGetTile,
            actionOnRelease: OnReleaseTile,
            actionOnDestroy: OnDestroyTile,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
        
        // Pré-instancier defaultCapacity tiles (optionnel)
        if (prewarmOnStart)
        {
            StartCoroutine(PrewarmPoolGradually());
        }
    }
    
    // Pré-instancier les tiles progressivement pour éviter le freeze
    private IEnumerator PrewarmPoolGradually()
    {
        GameObject[] prewarmTiles = new GameObject[defaultCapacity];
        
        // Créer par petits lots de 10 tiles par frame
        int batchSize = 10;
        for (int i = 0; i < defaultCapacity; i++)
        {
            prewarmTiles[i] = pool.Get();
            
            // Attendre une frame après chaque lot
            if ((i + 1) % batchSize == 0)
            {
                yield return null;
            }
        }
        
        // Retourner toutes les tiles au pool
        for (int i = 0; i < defaultCapacity; i++)
        {
            pool.Release(prewarmTiles[i]);
        }
    }
    
    // Créer une nouvelle tile
    private GameObject CreateTile()
    {
        GameObject tile = Instantiate(tilePrefab);
        tile.transform.SetParent(transform); // Organiser dans la hiérarchie
        return tile;
    }
    
    // Quand une tile est prise du pool
    private void OnGetTile(GameObject tile)
    {
        tile.SetActive(true);
    }
    
    // Quand une tile est retournée au pool
    private void OnReleaseTile(GameObject tile)
    {
        tile.SetActive(false);
    }
    
    // Quand une tile est détruite (pool plein)
    private void OnDestroyTile(GameObject tile)
    {
        Destroy(tile);
    }
    
    // Méthodes publiques pour obtenir/retourner des tiles
    public GameObject GetTile()
    {
        return pool.Get();
    }
    
    public void ReleaseTile(GameObject tile)
    {
        pool.Release(tile);
    }
    
    // Pour le debug
    public int CountActive => pool.CountActive;
    public int CountInactive => pool.CountInactive;
    public int CountAll => pool.CountAll;
}

