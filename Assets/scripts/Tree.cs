using System;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private GrowComponent growComponent;
    
    private void Awake()
    {
        growComponent = GetComponentInChildren<GrowComponent>();
    }
    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }
    
    private void OnTick()
    {
        growComponent.Grow();
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}