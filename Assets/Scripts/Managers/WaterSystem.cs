using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    public static WaterSystem instance { get; private set;}
    public HashSet<WaterSource> waterSources { get; private set; } = new();
    public HashSet<GameObject> waterFlows { get; private set; } = new();
    [field: SerializeField] public GameObject waterFlowPrefab { get; private set; }
    [field: SerializeField] public float maxFlowDistance { get; private set; }

    private void Awake()
    {
        if ( instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        WaterSource.waterSpawned += OnWaterSpawned;
        WaterSource.waterRemoved += OnWaterRemoved;
        TilemapManager.instance.tileRemoved += EnableNeighbors;
        TickSystem.ticked += OnTicked;
    }

    private void OnWaterRemoved(WaterSource waterSource)
    {
        waterSources.Remove(waterSource);
    }

    private void OnTicked()
    {
        foreach (WaterSource water in waterSources.ToList())
        {
            water.Expand();
        }
    }

    private void OnDisable()
    {
        WaterSource.waterSpawned -= OnWaterSpawned;
        WaterSource.waterRemoved -= OnWaterRemoved;
        TickSystem.ticked -= OnTicked;
    }
    
    private void OnWaterSpawned(WaterSource waterSource)
    {
        waterSources.Add(waterSource);
    }
    
    public void EnableNeighbors(Vector3Int coordinate)
    {
        Vector3Int downCoords = coordinate + new Vector3Int(0, 0, -1);
        GameObject underTile = TilemapManager.instance.GetTile(downCoords);
        if (downCoords.z > 0 && underTile == null)
        {
            WaterSource underWater = underTile?.GetComponent<WaterSource>();
            if (underWater != null)
            {
                underWater.Toggle(true);
            }
        }

        foreach (Vector3Int offset in WaterSource.horizontalOffsets)
        {
            Vector3Int neighborCoords = coordinate + offset;

            if (neighborCoords.z < 0)
            {
                continue;
            }

            GameObject neighborTile = TilemapManager.instance.GetTile(neighborCoords);
            if (neighborTile == null)
            {
                continue;
            }
            WaterSource neighborWater = neighborTile?.GetComponent<WaterSource>();
            if (neighborWater == null)
            {
                continue;
            }
            neighborWater.Toggle(true);
        }
    }
}