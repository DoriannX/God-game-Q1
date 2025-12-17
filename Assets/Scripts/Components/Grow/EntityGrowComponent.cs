using System.Collections.Generic;
using UnityEngine;

public class EntityGrowComponent : GrowComponent {
    [SerializeField] private List<AnimationClip> growthStages;
    private Animator animator;
    
    public override void Grow() {
        
    }
}
