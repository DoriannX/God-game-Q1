using System;
using SaveLoadSystem;
using UnityEngine;
[RequireComponent(typeof(SaveableEntity))]
public class GrowComponent : MonoBehaviour, ISaveable
{
    [Serializable]
    public struct GrowData
    {
        public float growthProgressPercent;
    }
    public float growthProgressPercent { get; private set; } = 0f;
    [SerializeField] private float growthRate = 0.1f;
    [SerializeField] private Sprite[] growthStages;
    private SpriteRenderer spriteRenderer;
    public event Action onFullyGrown;
    
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (growthStages.Length > 0) {
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

    public bool NeedsToBeSaved()
    {
        return true;
    }

    public bool NeedsReinstantiation()
    {
        return false;
    }

    public object SaveState()
    {
        var data = new GrowData
        {
            growthProgressPercent = growthProgressPercent
        };
        return data;
    }

    public void LoadState(object state)
    {
        var data = (GrowData)state;
        growthProgressPercent = data.growthProgressPercent;
        int stageIndex = Mathf.Min((int)(growthProgressPercent * growthStages.Length), growthStages.Length - 1);
        spriteRenderer.sprite = growthStages[stageIndex];
    }

    public void PostInstantiation(object state)
    {
    }

    public void GotAddedAsChild(GameObject obj, GameObject hisParent)
    {
    }
}
