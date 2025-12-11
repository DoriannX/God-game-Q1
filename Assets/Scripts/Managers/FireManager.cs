using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireManager : MonoBehaviour
{  
    private HashSet<Vector3Int> burningTiles = new HashSet<Vector3Int>();
    
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private Vector3Int whereFireSpawn; //Remove when have button
    public float lifeTime;
    [SerializeField, Range(0, 100)] private int propabilityToSpread;
    [HideInInspector] public int propability;
    
    public List<Fire> fireList = new List<Fire>();
    public static FireManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        propability = propabilityToSpread;
        Vector3 spawnPoint = whereFireSpawn;
        int topZ = TilemapManager.instance.GetColumnTopCoordinate(new Vector2Int(whereFireSpawn.x, whereFireSpawn.z));
        spawnPoint.y = topZ * 0.2f;
        
        Instantiate(firePrefab, spawnPoint, Quaternion.identity);
    }
    
    public void UnregisterFire(Fire fire)
    {
        fireList.Remove(fire);
        propability--;
        Vector3Int tile = TilemapManager.instance.WorldToHexAxial(fire.transform.position);
        burningTiles.Remove(tile);
    }
}