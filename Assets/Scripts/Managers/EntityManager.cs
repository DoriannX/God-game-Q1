using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Unity.Behavior;
using UnityEngine;

public class EntityManager : MonoBehaviour {
    public static EntityManager instance { get; private set; }
    EntityAIOptimizer entityOptimizer;
    
    [SerializeField] private int maxEntityPerType = 100;
    
    [SerializeField] SerializedDictionary<EntityType, GameObject> entityPrefabs;
    Dictionary<EntityType, HashSet<EntityAI>> entities = new();
    Dictionary<EntityType, GameObject> entitiesParent = new();
    
    public event Action<EntityType, int> onEntityChanged;
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        entityOptimizer = GetComponent<EntityAIOptimizer>();
    }

    public GameObject SpawnEntity(EntityType entityType, Vector3 position) {
        if (!entities.ContainsKey(entityType)) {
            // Create a new parent GameObject for this entity type as a child of this Manager
            GameObject newParent = new GameObject(entityType.ToString());
            newParent.transform.SetParent(transform);
            entitiesParent[entityType] = newParent;
            entities[entityType] = new HashSet<EntityAI>();
        }
        
        if (!entities.TryGetValue(entityType, out HashSet<EntityAI> entitySet)) {
            return null;
        }
        
        if (entitySet.Count >= maxEntityPerType) {
            Debug.LogWarning("Max entities of type " + entityType + " reached");
            return null;
        }
        
        // Instantiate entity as a child of its parent
        GameObject entity = Instantiate(entityPrefabs[entityType], position, Quaternion.identity, entitiesParent[entityType].transform);
        entitySet.Add(entity.GetComponent<EntityAI>());
        onEntityChanged?.Invoke(entityType, entitySet.Count);
        entityOptimizer.RegisterAgent(entity.GetComponent<BehaviorGraphAgent>());
        return entity;
    }

    public void RemoveEntity(EntityAI entity) {
        if (!entities.TryGetValue(entity.entityType, out HashSet<EntityAI> entitySet)) {
            return;
        }
        entitySet.Remove(entity);
        Destroy(entity.gameObject);
        onEntityChanged?.Invoke(entity.entityType, entitySet.Count);
        entityOptimizer.UnregisterAgent(entity.GetComponent<BehaviorGraphAgent>());
    }
    
    public void RegisterEntity(EntityAI entity) {
        if (!entities.ContainsKey(entity.entityType)) {
            entities[entity.entityType] = new HashSet<EntityAI>();
        }
        entities[entity.entityType].Add(entity);
        onEntityChanged?.Invoke(entity.entityType, entities[entity.entityType].Count);
    }

    public void RemoveEntitiesInHouse(House house) {
        /*for (int numberOfEntityInHouse = 0; numberOfEntityInHouse < house.fuckingEntities.Count; numberOfEntityInHouse++) {
            entities.Remove(house.fuckingEntities.ElementAt(numberOfEntityInHouse).entityType);
        }
        Destroy(house.gameObject);
        onEntityChanged?.Invoke(entity.entityType, entities[entity.entityType].Count);*/
    }
}
