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
    }
        
    public override void OnLogic()
    {
        base.OnLogic();
        
    }
        
    public override void OnExit()
    {
        base.OnExit();
    }
}