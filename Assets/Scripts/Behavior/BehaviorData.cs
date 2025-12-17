using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BehaviorData : MonoBehaviour {
    [field : SerializeField] public WanderComponent wanderComponent { get; private set; }
    [field : SerializeField] public WorkComponent workComponent { get; private set; }
    [field : SerializeField] public BreedComponent breedComponent { get; private set; }
    [field : SerializeField] public EntityAI EntityAI { get; private set; }
    [field : SerializeField] public EntityGrow entityGrow { get; private set; }
    [SerializeField] private float workProbability;
    [SerializeField] private float breedProbability;

    public bool CheckWork() => Random.Range(0f, 1f) < workProbability;
    public bool CheckBreed () => Random.Range(0f, 1f) < breedProbability;
    
    public bool CheckGrown() => entityGrow.isFullyGrown;
    
    public bool CheckWorkFinished() => workComponent.isWorkDone;
    public bool CheckBreedFinished() => breedComponent.isBreedDone || breedComponent.isBreedFailed;

}
