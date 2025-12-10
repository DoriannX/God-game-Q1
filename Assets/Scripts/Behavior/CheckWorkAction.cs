using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CheckWork", story: "Check if [AI] go work", category: "Action", id: "3c71840a1138b18d36e1cd9f775b7434")]
public partial class CheckWorkAction : Action
{
    [SerializeReference] public  BlackboardVariable<bool> Gyat;
    [SerializeReference] public  BlackboardVariable<BehaviorData> AI;

    protected override Status OnStart() {
        Gyat.Value = AI.Value.CheckWork();
        return Status.Success;
    }

}

