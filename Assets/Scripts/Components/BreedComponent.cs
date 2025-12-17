using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BreedComponent : MonoBehaviour {
    public bool isBreedDone { get; private set; }
    public bool isBreedFailed { get; private set; }
    public event Action onBreedDone;
    public event Action onBreedFailed;
    [SerializeField] private EntityAI entityAI;
        
    private readonly Collider2D[] colliders = new Collider2D[100];
    [SerializeField] private float range;

    private void Awake() {
        onBreedDone += () => { isBreedDone = true; };
        onBreedFailed += () => { isBreedFailed = true; };
    }

    public House GetNearestHouse(Vector2 position, EntityType entityType) {
        House nearestHouse = null;
        float nearestDistance = float.MaxValue;
        
        ContactFilter2D filter = new ContactFilter2D();
        
        int count = Physics2D.OverlapCircle(position, range, filter, colliders);
        for (int i = 0; i < count; i++) {
            House house = colliders[i].GetComponent<House>();
            if (house == null) {
                continue;
            }

            if (house.isFull || (house.breedEntity != entityType && !house.isEmpty)) {
                continue;
            }
            float distance = Vector2.Distance(position, house.transform.position);
            if (distance >= nearestDistance) {
                continue;
            }
            nearestDistance = distance;
            nearestHouse = house;
        }
        return nearestHouse;
    }

    public void GoBreed() {
        House house = GetNearestHouse(transform.position, entityAI.GetEntityType());
        if (house == null) {
            onBreedFailed?.Invoke();
            return;
        }
        house.onBreedFinished += () => onBreedDone?.Invoke();
        entityAI.GoTo(house.transform.position);
        if (Vector2.Distance(entityAI.transform.position, house.transform.position) < 0.1f) {
            house.Enter(entityAI, onBreedFailed);
        }
    }
}