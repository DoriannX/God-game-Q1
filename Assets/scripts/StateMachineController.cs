using System;
using UnityEngine;
using UnityHFSM;

public class StateMachineController : MonoBehaviour
{
    [SerializeField] private WanderComponent wanderComponent;
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
        stateMachine.AddState("Wander", wanderState);
        stateMachine.SetStartState("Wander");
        stateMachine.Init();
    }

    private void Tick()
    {
        stateMachine.OnLogic();
    }
}
