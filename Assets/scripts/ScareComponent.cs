using System;
using System.Collections.Generic;
using UnityEngine;

public class ScareComponent : MonoBehaviour
{
    public event Action onScare;
    private List<Vector2> scarePoints = new();
    public void Scare(Vector2 point)
    {
        scarePoints.Add(point);
        onScare?.Invoke();
    }

    public Vector2 GetScareDirection()
    {
        Vector2 direction = Vector2.zero;
        foreach (var point in scarePoints)
        {
            direction += ((Vector2)transform.position - point).normalized;
        }
        return direction.normalized;
    }
}
