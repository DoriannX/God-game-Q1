using UnityEngine;
using UnityHFSM;
using Random = UnityEngine.Random;

public class StateMachineController : MonoBehaviour
{
    //TODO: Save
    [SerializeField] private WanderComponent wanderComponent;
    [SerializeField] private WorkComponent workComponent;
    [SerializeField] private HouseRetriever houseRetriever;
    [SerializeField] private GrowComponent growComponent;
    [SerializeField] private ScareComponent scareComponent;
    [SerializeField] private GhostIa ghostIA;
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
        growComponent.onFullyGrown += SetupStateMachine;
        stateMachine = new StateMachine();
        State wanderState = new WanderState(wanderComponent);
        State scareState = new ScareState(scareComponent, ghostIA);
        stateMachine.AddTriggerTransitionFromAny("OnScare", "Scare");
        stateMachine.AddState("Scare", scareState);
        scareComponent.onScare += () =>
        {
            stateMachine.Trigger("OnScare"); 
        };
        stateMachine.AddState("Wander", wanderState);
        stateMachine.SetStartState("Wander");
        stateMachine.Init();
    }   
    private void SetupStateMachine()
    {
        print("SetupStateMachine");
        State workState = new WorkState( ghostIA, workComponent);
        State fuckState = new FuckState(ghostIA, houseRetriever);
        ((FuckState) fuckState).onFuckFinished += () => stateMachine.Trigger("OnFuckFinished");
        workComponent.onWork += () => stateMachine.Trigger("OnWork");
        stateMachine.AddState("Work", workState);
        stateMachine.AddState("Fuck", fuckState);
        stateMachine.AddTransition("Wander", "Work", _ => Random.Range(0f, 1f) < 0.1f);
        stateMachine.AddTransition("Wander", "Fuck", _ => Random.Range(0f, 1f) < 0.05f);
        stateMachine.AddTriggerTransition("OnWork", "Work", "Wander");
        stateMachine.AddTriggerTransition("OnFuckFinished", "Fuck", "Wander");
        stateMachine.Init();
    }

    private void Tick()
    {
        stateMachine.OnLogic();
    }
}
