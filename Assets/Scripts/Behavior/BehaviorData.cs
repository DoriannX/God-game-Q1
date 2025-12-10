using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BehaviorData : MonoBehaviour {
    [field : SerializeField] public WanderComponent wanderComponent { get; private set; }
    [field : SerializeField] public WorkComponent workComponent { get; private set; }
    [field : SerializeField] public HouseRetriever houseRetriever { get; private set; }
    [field : SerializeField] public ScareComponent scareComponent { get; private set; }
    [field : SerializeField] public EntityIA entityIa { get; private set; }
    [field : SerializeField] public EntityGrow entityGrow { get; private set; }
    [SerializeField] private float workProbability;
    [SerializeField] private float fuckProbability;

    public bool CheckWork() => Random.Range(0f, 1f) < workProbability;
    public bool CheckFuck () => Random.Range(0f, 1f) < fuckProbability;
    
    public bool CheckGrown() => entityGrow.isFullyGrown;
    
    public bool CheckWorkFinished() => workComponent.isWorkDone;

}
