using UnityEngine;
using UnityHFSM;

public class WorkState : State
{
    private GhostAI ghostAI;
    private WorkComponent workComponent;
    public WorkState(GhostAI ghostAI, WorkComponent workComponent)
    {
        this.workComponent = workComponent;
        this.ghostAI = ghostAI;
    }

    /*public override void OnLogic()
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
    }*/
}