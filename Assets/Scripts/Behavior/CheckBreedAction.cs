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

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

