using System;
using UnityEngine;

public class GrowComponent : MonoBehaviour
{
    private float growthProgressPercent = 0f;
    [SerializeField] private float growthRate = 0.1f;
    [SerializeField] private Sprite[] growthStages;
    private SpriteRenderer spriteRenderer;
    public event Action onFullyGrown;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (growthStages.Length > 0)
        {
            spriteRenderer.sprite = growthStages[0];
        }
    }

    public void Grow()
    {
        growthProgressPercent += growthRate;
        int stageIndex = Mathf.Min((int)(growthProgressPercent * growthStages.Length), growthStages.Length - 1);
        spriteRenderer.sprite = growthStages[stageIndex];

        if (growthProgressPercent >= 1f)
        {
            onFullyGrown?.Invoke();
        }
    }
}
