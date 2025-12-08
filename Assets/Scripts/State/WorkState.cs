using UnityEngine;
using UnityHFSM;

public class WorkState : State
{
    private GhostIa ghostIA;
    private WorkComponent workComponent;
    public WorkState(GhostIa ghostIA, WorkComponent workComponent)
    {
        this.workComponent = workComponent;
        this.ghostIA = ghostIA;
    }

    public override void OnLogic()
    {
        base.OnLogic();
        WorkTask task = workComponent.GetTask();
        if (task == null) {
            workComponent.Work();
            return;
        }
        ghostIA.GoTo(task.transform.position);
        if(Vector2.Distance(ghostIA.transform.position, workComponent.GetTask().transform.position) < 0.1f)
        {
            workComponent.Work();
        }
    }
}