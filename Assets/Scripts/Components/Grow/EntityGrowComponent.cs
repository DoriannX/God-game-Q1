using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityGrowComponent : GrowComponent {
    [SerializeField] private List<AnimationClip> growthStages;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        if (growthStages.Count > 0) {
            animator.Play(growthStages[0].name);
        }
    }

    public override void Grow() {
        growthProgressPercent += growthRate;
        int stageIndex = Mathf.Min((int)(growthProgressPercent * growthStages.Count), growthStages.Count - 1);
        animator.Play(growthStages[stageIndex].name);

        if (growthProgressPercent >= 1f) {
            DoneGrowing();
        }
    }
}
