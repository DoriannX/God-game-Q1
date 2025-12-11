using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GrowCheck", story: "Check if [AI] is done growing", category: "Action", id: "8f80bcda0944c6b0387aba2342c2e922")]
public partial class GrowCheckAction : Action {
    [SerializeReference] public BlackboardVariable<BehaviorData> AI;
    protected override Status OnStart() {
        return AI.Value.CheckGrown() ? Status.Failure : Status.Success;
    }
}