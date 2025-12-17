using System;
using UnityEngine;

public abstract class GrowComponent : MonoBehaviour {
    public float growthProgressPercent { get; protected set; } = 0f;
    [SerializeField] protected float growthRate = 0.1f;
    
    public event Action onFullyGrown;
    
    public abstract void Grow();
    
    protected void DoneGrowing() => onFullyGrown?.Invoke();
}
