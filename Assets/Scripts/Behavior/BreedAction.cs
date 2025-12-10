using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Breed", story: "[AI] go breeding", category: "Action", id: "591db81f7e463f03816edd69ecf7007c")]
public partial class BreedAction : Action {
    [SerializeReference] public BlackboardVariable<BehaviorData> AI;

    protected override Status OnStart()
    {
        
        return Status.Running;
    }
}

