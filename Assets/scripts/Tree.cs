using System;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private float growthProgressPercent = 0f;
    [SerializeField] private float growthRate = 0.1f;
    [SerializeField] private Sprite[] growthStages;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (growthStages.Length > 0)
        {
            spriteRenderer.sprite = growthStages[0];
        }
    }
    private void OnEnable()
    {
        TickSystem.ticked += OnTick;
        growthProgressPercent = 0f;
        if (growthStages.Length > 0)
        {
            spriteRenderer.sprite = growthStages[0];
        }
        
    }

    private void OnTick()
    {
        growthProgressPercent += growthRate;
        int stageIndex = Mathf.Min((int)(growthProgressPercent * growthStages.Length), growthStages.Length - 1);
        spriteRenderer.sprite = growthStages[stageIndex];
        if (growthProgressPercent >= 1f)
        {
            TickSystem.ticked -= OnTick;
        }
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= OnTick;
    }
}