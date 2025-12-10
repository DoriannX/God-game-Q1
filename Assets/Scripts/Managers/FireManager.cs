using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;

    [Header("Prefab")]
    public GameObject firePrefab;
    
    public HashSet<Vector3Int> burningTiles = new HashSet<Vector3Int>();


    public List<Fire> fireList = new List<Fire>();

    private void Awake()
    {
        Instance = this;
    }
    
    public bool TrySpawnFire(Vector3Int tile)
    {
        if (!burningTiles.Add(tile))
            return false;
        
        Vector3 worldPos = TilemapManager.instance.HexAxialToWorld(tile);
        
        Instantiate(firePrefab, worldPos, Quaternion.identity);

        return true;
    }
    public void SpawnGameObjectButton()
    {
        Instantiate(firePrefab, TilemapManager.instance.currentHexCoordinates, Quaternion.identity);
    }
    
    public void UnregisterFire(Fire fire)
    {
        fireList.Remove(fire);

        Vector3Int tile = TilemapManager.instance.WorldToHexAxial(fire.transform.position);
        burningTiles.Remove(tile);
    }
}