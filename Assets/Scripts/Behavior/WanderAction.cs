using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wander", story: "[AI] wander", category: "Action", id: "cee6a87fe0b65e43aa4642688af8aa6b")]
public partial class WanderAction : Action {
    [SerializeReference] public BlackboardVariable<BehaviorData> AI;
    protected override Status OnStart() {
        AI.Value.wanderComponent.Wander();
        return Status.Success;
    }
}

