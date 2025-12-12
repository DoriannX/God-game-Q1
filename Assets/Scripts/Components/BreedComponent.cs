using System;
using UnityEngine;

public class BreedComponent : MonoBehaviour {
    public bool isBreedDone { get; private set; }
    public event Action onBreedDone;
    [SerializeField] private EntityIA entityIa;
        
    private readonly Collider2D[] colliders = new Collider2D[100];
    [SerializeField] private float range;

    private void Awake() {
        onBreedDone += () =>
        {
            Debug.Log("BreedFinish");
            isBreedDone = true;
        };
    }

    public House GetNearestHouse(Vector2 position, EntityType entityType) {
        House nearestHouse = null;
        float nearestDistance = float.MaxValue;
        
        ContactFilter2D filter = new ContactFilter2D();
        
        int count = Physics2D.OverlapCircle(position, range, filter, colliders);
        for (int i = 0; i < count; i++) {
            var house = colliders[i].GetComponent<House>();
            if (house == null) {
                continue;
            }

            if (house.isFull) {
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
        Debug.Log("Go Breed");
        House house = GetNearestHouse(transform.position, entityIa.entityType);
        if (house == null) {
            onBreedDone?.Invoke();
            return;
        }
        house.onBreedFinished += () => onBreedDone?.Invoke();
        entityIa.GoTo(house.transform.position);
        if (Vector2.Distance(entityIa.transform.position, house.transform.position) < 0.1f) {
            house.Enter(entityIa);
        }
    }
}
