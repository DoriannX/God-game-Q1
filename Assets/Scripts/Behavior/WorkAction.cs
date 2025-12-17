using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Work", story: "[AI] go working", category: "Action", id: "15790155747800ad5ee970caaa9f8431")]
public partial class WorkAction : Action {
    [SerializeReference] public BlackboardVariable<BehaviorData> AI;
    protected override Status OnStart() {
        Debug.Log("Go Work");
        WorkTask task = AI.Value.workComponent.GetTask();
        if (task == null) {
            AI.Value.workComponent.Work();
            return Status.Failure;
        }
        AI.Value.EntityAI.GoTo(task.transform.position);
        if(Vector2.Distance( AI.Value.EntityAI.transform.position, AI.Value.workComponent.GetTask().transform.position) < 0.1f) {
            AI.Value.workComponent.Work();
        }
        return Status.Failure;
    }

    protected override Status OnUpdate() {

        
        return AI.Value.CheckWorkFinished() ? Status.Success : Status.Running;
    }
}

