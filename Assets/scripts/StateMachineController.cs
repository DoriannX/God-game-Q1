using System;
using UnityEngine;
using UnityHFSM;
using Random = UnityEngine.Random;


public class StateMachineController : MonoBehaviour
{
    [SerializeField] private WanderComponent wanderComponent;
    [SerializeField] private WorkComponent workComponent;
    [SerializeField] private GhostIA ghostIA;
    private StateMachine stateMachine;

    private void OnEnable()
    {
        TickSystem.ticked += Tick;
    }
    
    private void OnDisable()
    {
        TickSystem.ticked -= Tick;
    }

    private void Awake()
    {
        stateMachine = new StateMachine();
        State wanderState = new WanderState(wanderComponent);
        State workState = new WorkState( ghostIA, workComponent);
        workComponent.onWork += () => stateMachine.Trigger("OnWork");
        stateMachine.AddState("Wander", wanderState);
        stateMachine.AddState("Work", workState);
        stateMachine.AddTransition("Wander", "Work", _ => Random.Range(0f, 1f) < 0.1f);
        stateMachine.AddTriggerTransition("OnWork", "Work", "Wander");
        stateMachine.SetStartState("Wander");
        stateMachine.Init();
    }

    private void Tick()
    {
        stateMachine.OnLogic();
    }
}
