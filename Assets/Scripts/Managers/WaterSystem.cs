using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;

public class WaterSystem : MonoBehaviour
{
    public static WaterSystem instance { get; private set;}
    private HashSet<WaterComponent> waterBodies = new();
    
    public HashSet<WaterComponent> WaterBodies => waterBodies;

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
        WaterComponent.waterSpawned += OnWaterSpawned;
        WaterComponent.waterRemoved += OnWaterRemoved;
        TilemapManager.instance.tileRemoved += EnableNeighbors;
        TickSystem.ticked += OnTicked;
    }

    private void OnWaterRemoved(WaterComponent water)
    {
        waterBodies.Remove(water);
    }

    private void OnTicked()
    {
        foreach (WaterComponent water in waterBodies.ToList())
        {
            water.Expand();
        }
    }

    private void OnDisable()
    {
        WaterComponent.waterSpawned -= OnWaterSpawned;
        WaterComponent.waterRemoved -= OnWaterRemoved;
        TickSystem.ticked -= OnTicked;
    }
    
    private void OnWaterSpawned(WaterComponent water)
    {
        waterBodies.Add(water);
    }
    
    public void EnableNeighbors(Vector3Int coordinate)
    {
        Vector3Int downCoords = coordinate + new Vector3Int(0, 0, -1);
        GameObject underTile = TilemapManager.instance.GetTile(downCoords);
        if (downCoords.z > 0 && underTile == null)
        {
            WaterComponent underWater = underTile?.GetComponent<WaterComponent>();
            if (underWater != null)
            {
                underWater.Toggle(true);
            }
        }

        foreach (Vector3Int offset in WaterComponent.horizontalOffsets)
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
            WaterComponent neighborWater = neighborTile?.GetComponent<WaterComponent>();
            if (neighborWater == null)
            {
                continue;
            }
            neighborWater.Toggle(true);
        }
    }
}