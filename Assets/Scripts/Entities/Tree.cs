using System;
using SaveLoadSystem;
using UnityEngine;

public class Tree : MonoBehaviour {
    public ObjectGrowComponent growComponent;
    private float waterProgress = 0;
    [SerializeField] private float waterIncrement = 0.1f;
    
    private void Awake()
    {
        growComponent = GetComponentInChildren<ObjectGrowComponent>();
    }
    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
    }
    
    private void OnTick()
    {
        if(MeteoManager.Instance.state == MeteoState.Raining)
        {
            waterProgress += waterIncrement;
        }
        else
        {
            waterProgress -= waterIncrement / 2;
        }

        if (!(waterProgress >= 1))
        {
            return;
        }
        waterProgress = 0;
        if(growComponent == null)
        {
            growComponent = GetComponentInChildren<ObjectGrowComponent>();
        }
        growComponent.Grow();
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}