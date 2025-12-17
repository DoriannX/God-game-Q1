using System;
using System.Collections.Generic;
using System.Linq;
using SaveLoadSystem;
using UnityEngine;

[RequireComponent(typeof(SaveableEntity))]
public class House : WorkTask {
    public HashSet<EntityAI> breedingEntities = new();
    private ObjectGrowComponent growComponent;
    private float breedProgress = 0;
    [SerializeField] private float breedIncrement = 0.1f;
    [SerializeField] private int tickToExitAlone = 5;
    [SerializeField] private int minBabies = 1;
    [SerializeField] private int maxBabies = 3;
    public EntityType breedEntity;
    private Animator animator;
    private int ticksAlone;
    private bool isBreeding;
    public bool isFull => breedingEntities.Count >= 2;
    public bool isEmpty => breedingEntities.Count == 0;
    public event Action onBreedFinished;


    private void Awake() {
        growComponent = GetComponentInChildren<ObjectGrowComponent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable() {
        TickSystem.ticked += OnTicked;
    }

    public override void Work() {
        base.Work();
        growComponent.Grow();
    }

    protected override void OnComplete() { }

    public void Enter(EntityAI entityAI, Action onBreedFailed = null) {
        if (breedingEntities.Count == 0) {
            breedEntity = entityAI.GetEntityType();
        }
        else {
            if (breedEntity != entityAI.GetEntityType()) {
                onBreedFailed?.Invoke();
                return;
            }
        }
        breedingEntities.Add(entityAI);
        entityAI.gameObject.SetActive(false);
        if (breedingEntities.Count == 2)
            StartBreeding();
    }

    private void StartBreeding() {
        if (breedingEntities.Count < 2) return;
        animator.Play("HouseSexAnimation");
        isBreeding = true;
    }

    private void OnTicked() {
        if (breedingEntities.Count == 1) {
            ticksAlone++;
            if (ticksAlone >= tickToExitAlone) {
                Exit(breedingEntities.First());
                ticksAlone = 0;
            }
        }
        else {
            ticksAlone = 0;
        }

        if (!isBreeding) {
            return;
        }

        breedProgress += breedIncrement;
        if (breedProgress < 1f) {
            return;
        }

        int babiesCount = minBabies + (int)((maxBabies - minBabies) * progress);
        for (int i = 0; i < babiesCount; i++) {
            EntityManager.instance.SpawnEntity(breedEntity,TilemapManager.instance.HexAxialToWorld(
                TilemapManager.instance.WorldToHexAxial(transform.position)));
        }

        breedProgress = 0f;
        isBreeding = false;

        animator.Play("Idle");
        foreach (EntityAI ghost in breedingEntities.ToList()) {
            Exit(ghost);
        }
    }

    private void Exit(EntityAI entity) {
        
        breedingEntities.Remove(entity);
        entity.gameObject.SetActive(true);
        onBreedFinished?.Invoke();
    }
    
    private void OnDisable() {
        TickSystem.ticked -= OnTicked;
    }

    private void OnDestroy() {
        EntityManager.instance.RemoveEntitiesInHouse(this);
    }
}