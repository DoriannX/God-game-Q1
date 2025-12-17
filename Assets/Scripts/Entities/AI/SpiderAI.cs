using UnityEngine;

public class SpiderAI : EntityAI
{
    public override EntityType GetEntityType() {
        return EntityType.Spider;
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
}