using System;
using System.Collections.Generic;
using SaveLoadSystem;
using UnityEngine;

public class DevilAI : EntityAI
{
    public override EntityType entityType { get; protected set; } = EntityType.Devil ;
    
    public void ForceRepath() => ComputePath();
    
}