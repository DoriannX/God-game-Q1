using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;

public class GhostIa : EntityIA
{
    public override EntityType entityType { get; protected set; } = EntityType.Ghost ;
    
    [Serializable]
    public struct GhostData
    {
        public SaveableEntity.Vector2Data position;
        public SaveableEntity.Vector2Data targetPosition;
        public List<SaveableEntity.Vector2Data> currentPath;
    }
    
    public void ForceRepath() => ComputePath();
    
    private void OnDrawGizmos() {
        if (currentPath == null || currentPath.Count == 0) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
        }
    }

    public override object SaveState() {
        var data = new GhostData {
            position = new SaveableEntity.Vector2Data(transform.position),
            targetPosition = new SaveableEntity.Vector2Data(targetPosition),
            currentPath = new List<SaveableEntity.Vector2Data>()
        };
        if (currentPath != null) {
            foreach (var pos in currentPath) {
                data.currentPath.Add(new SaveableEntity.Vector2Data(pos));
            }
        }
        return data;
    }

    public override void LoadState(object state) {
        var data = (GhostData)state;
        transform.position = data.position.ToVector2();
        targetPosition = data.targetPosition.ToVector2();
        currentPath = new List<Vector3>();
        if (data.currentPath != null) {
            foreach (var pos in data.currentPath) {
                currentPath.Add(pos.ToVector2());
            }
        }
        else {
            currentPath = null;
        }
        entityManager.RegisterEntity(this);
    }

}