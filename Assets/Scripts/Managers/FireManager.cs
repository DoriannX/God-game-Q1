using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;

    public Vector3Int whereFireSpawn; //Remove when have button

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
    public void SpawnGameObjectButton() //Change when have button
    {
        Vector3 spawnPoint = whereFireSpawn;
        int topZ = TilemapManager.instance.GetColumnTopCoordinate(new Vector2Int(whereFireSpawn.x, whereFireSpawn.z));
        spawnPoint.y = topZ * 0.2f;
        
        Instantiate(firePrefab, spawnPoint, Quaternion.identity);

    }
    
    public void UnregisterFire(Fire fire)
    {
        fireList.Remove(fire);

        Vector3Int tile = TilemapManager.instance.WorldToHexAxial(fire.transform.position);
        burningTiles.Remove(tile);
    }
}