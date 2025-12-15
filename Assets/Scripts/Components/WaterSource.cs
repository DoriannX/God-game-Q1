using System;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class WaterSource : MonoBehaviour
    {
        public static event Action<WaterSource> waterSpawned;
        public static event Action<WaterSource> waterRemoved;
        private HashSet<WaterFlow> connectedFlows = new();
        private bool activated = true;
        int expandedTimes = 0;

        private void OnEnable()
        {
            waterSpawned?.Invoke(this);
        }

        private void OnDisable()
        {
            waterRemoved?.Invoke(this);
        }

        public static Vector3Int[] horizontalOffsets =
        {
            new(1, 0, 0), // Right
            new(1, -1, 0), // Top Right
            new(0, -1, 0), // Top Left
            new(-1, 0, 0), // Left
            new(-1, 1, 0), // Bottom Left
            new(0, 1, 0) // Bottom Right
        };

        public void Toggle(bool state)
        {
            activated = state;
        }

        public void Expand()
        {
            
            if (!activated)
                return;

            Vector3Int currentHexCoords = TilemapManager.instance.WorldToHexAxial(transform.position);

            Vector3Int downCoords = currentHexCoords + new Vector3Int(0, 0, -1);
            GameObject underTile = TilemapManager.instance.GetTile(downCoords);

            if (downCoords.z > TilemapManager.instance.tileHeight && underTile == null)
            {
                TilemapManager.instance.SpawnTileAt(downCoords, gameObject);
                activated = false;
                return;
            }

            // Check for null water flows (bucketed water) and respawn them
            HashSet<WaterFlow> nullFlows = new HashSet<WaterFlow>();
            foreach (WaterFlow flow in connectedFlows)
            {
                if (flow == null)
                {
                    nullFlows.Add(flow);
                }
            }

            // Remove null entries and respawn water at those positions
            foreach (WaterFlow nullFlow in nullFlows)
            {
                connectedFlows.Remove(nullFlow);
            }

            // Check all connected flows and make them fall down infinitely until hitting ground or tile
            List<WaterFlow> newFlowsToAdd = new List<WaterFlow>();
            foreach (WaterFlow flow in connectedFlows)
            {
                if (flow == null) continue;

                Vector3Int flowHexCoords = TilemapManager.instance.WorldToHexAxial(flow.transform.position);
                Vector3Int checkDownCoords = flowHexCoords + new Vector3Int(0, 0, -1);

                // Keep falling until we hit ground or another tile
                while (checkDownCoords.z >= 0 && TilemapManager.instance.GetTile(checkDownCoords) == null)
                {
                    // Spawn water tile below
                    GameObject newWaterTile = TilemapManager.instance.SpawnTileAt(checkDownCoords, WaterSystem.instance.waterFlowPrefab);
                    if (newWaterTile != null)
                    {
                        var newFlowComponent = newWaterTile.GetComponent<WaterFlow>();
                        if (newFlowComponent != null)
                        {
                            newFlowsToAdd.Add(newFlowComponent);
                        }
                    }

                    // Move down one more level
                    checkDownCoords = checkDownCoords + new Vector3Int(0, 0, -1);
                }
            }

            // Add all new flows after iteration to avoid collection modification exception
            foreach (WaterFlow newFlow in newFlowsToAdd)
            {
                connectedFlows.Add(newFlow);
            }

            if (expandedTimes >= WaterSystem.instance.maxFlowDistance)
            {
                return;
            }

            expandedTimes++;
            int currentRadius = expandedTimes;

            // Iterate through all hexagonal positions at the current radius
            // For axial coordinates, we need to check all q,r where:
            // -radius <= q <= radius AND -radius <= r <= radius AND -radius <= -q-r <= radius
            for (int q = -currentRadius; q <= currentRadius; q++)
            {
                for (int r = Mathf.Max(-currentRadius, -q - currentRadius); 
                     r <= Mathf.Min(currentRadius, -q + currentRadius); 
                     r++)
                {
                    // Skip the center tile (that's where the water source is)
                    if (q == 0 && r == 0)
                        continue;

                    // Only spawn tiles at the current ring distance, not all tiles within radius
                    // This prevents re-checking already spawned tiles
                    int distance = (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(-q - r)) / 2;
                    if (distance != currentRadius)
                        continue;

                    Vector3Int neighborCoords = currentHexCoords + new Vector3Int(q, r, 0);

                    if (neighborCoords.z < 0)
                    {
                        continue;
                    }

                    if (TilemapManager.instance.GetTile(neighborCoords) != null)
                    {
                        continue;
                    }

                    GameObject newTile = TilemapManager.instance.SpawnTileAt(neighborCoords, WaterSystem.instance.waterFlowPrefab);
                    if (newTile == null)
                    {
                        continue;
                    }
                    var flowComponent = newTile.GetComponent<WaterFlow>();
                    if (flowComponent != null)
                    {
                        connectedFlows.Add(flowComponent);
                    }
                }
            }
            
            
        }
    }
}