using System.Collections.Generic;
using UnityEngine;

public class ObjectGrowComponent : GrowComponent {
    
    [SerializeField] private List<Sprite> growthStages;
    private SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (growthStages.Count > 0) {
            spriteRenderer.sprite = growthStages[0];
        }
    }

    
    public override void Grow() {
        growthProgressPercent += growthRate;
        int stageIndex = Mathf.Min((int)(growthProgressPercent * growthStages.Count), growthStages.Count - 1);
        spriteRenderer.sprite = growthStages[stageIndex];

        if (growthProgressPercent >= 1f) {
            DoneGrowing();
        }
    }
}