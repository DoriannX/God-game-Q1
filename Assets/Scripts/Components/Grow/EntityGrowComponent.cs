using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EntityGrowComponent : GrowComponent {

    [SerializeField] private List<AnimatorController> growthStages;
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    public override void Grow() {
        growthProgressPercent += growthRate;
        int stageIndex = Mathf.Min((int)(growthProgressPercent * growthStages.Count), growthStages.Count - 1);
        
        animator.runtimeAnimatorController = growthStages[stageIndex];
        
        if (growthProgressPercent >= 1f) {
            DoneGrowing();
        }
    }
}
