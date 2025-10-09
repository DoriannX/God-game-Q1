using UnityEngine;
using UnityHFSM;

public class WanderState : State
{
    private WanderComponent wanderComponent;
    public WanderState(WanderComponent component)
    {
        wanderComponent = component;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Entering Wander State");
    }
        
    public override void OnLogic()
    {
        base.OnLogic();
        wanderComponent.Wander();
    }
        
    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exiting Wander State");
    }
}