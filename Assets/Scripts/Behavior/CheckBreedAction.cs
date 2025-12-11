using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckBreed", story: "Check if [AI] go breed", category: "Action", id: "689095c50868c774c5a30bedd6d618b6")]
public partial class CheckBreedAction : Action
{
    [SerializeReference] public BlackboardVariable<BehaviorData> AI;
    [SerializeReference] public BlackboardVariable<bool> result;

    protected override Status OnStart() {
        result.Value = AI.Value.CheckBreed();
        return Status.Success;
    }
}

