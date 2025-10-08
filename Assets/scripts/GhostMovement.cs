using System;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private List<Vector2> path = new List<Vector2>();
    private Vector2 currentTarget;
    public void GoTo(Vector2 position)
    {
        currentTarget = position;
    }
    
    public void GoTo(List<Vector2> positions)
    {
        path = positions;
        if (path == null || path.Count == 0) return;
        if (path.Count > 0)
        {
            currentTarget = path[0];
            path.RemoveAt(0);
        }
    }

    private void Update()
    {
        
        Vector2 currentPosition = transform.position;
        if (Vector2.Distance(currentPosition, currentTarget) < 0.01f)
        {
            if(path == null || path.Count == 0) return;
            if (path.Count > 0)
            {
                currentTarget = path[0];
                path.RemoveAt(0);
            }
        }
        transform.position = Vector2.MoveTowards(currentPosition, currentTarget, speed * Time.deltaTime);
    }
}
