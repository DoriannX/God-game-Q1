using System;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class WaterComponent : MonoBehaviour
    {
        public static event Action<WaterComponent> waterSpawned;
        public static event Action<WaterComponent> waterRemoved;
        private bool activated = true;

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
            new(1, 0, 0),    // Right
            new(1, -1, 0),   // Top Right
            new(0, -1, 0),   // Top Left
            new(-1, 0, 0),   // Left
            new(-1, 1, 0),   // Bottom Left
            new(0, 1, 0)     // Bottom Right
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

            foreach (Vector3Int offset in horizontalOffsets)
            {
                Vector3Int neighborCoords = currentHexCoords + offset;
                
                if (neighborCoords.z < 0)
                {
                    continue;
                }
                
                if (TilemapManager.instance.GetTile(neighborCoords) != null)
                {
                    continue;
                }
                
                TilemapManager.instance.SpawnTileAt(neighborCoords, gameObject);
            }
            activated = false;
        }
    }
}