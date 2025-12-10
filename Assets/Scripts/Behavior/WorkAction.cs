using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Work", story: "[IA] go work", category: "Action", id: "15790155747800ad5ee970caaa9f8431")]
public partial class WorkAction : Action {
    [SerializeReference] public BlackboardVariable<BehaviorData> IA;

    protected override Status OnStart() {
        WorkTask task = IA.Value.workComponent.GetTask();
        if (task == null) {
            IA.Value.workComponent.Work();
            return Status.Running;
        }
        IA.Value.entityIa.GoTo(task.transform.position);
        if(Vector2.Distance( IA.Value.entityIa.transform.position, IA.Value.workComponent.GetTask().transform.position) < 0.1f)
        {
            IA.Value.workComponent.Work();
        }
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd() {
    }
}

