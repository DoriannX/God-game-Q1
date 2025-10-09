using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DestructionComponent : MonoBehaviour
{
    [field:SerializeField] public List<DestructionObject> destructionObjects { get; private set; }
    [SerializeField] private WaterSystem waterSystem;
    private HeightManager heightManager;
    public int objectIndex { get; private set; }
    private Vector2 lastPos;
    private Vector2 lastHitPos;
    private bool canPlace = true;

    private void Start()
    {
        heightManager = HeightManager.instance;
    }

    private void OnEnable()
    {
        TickSystem.ticked += ResetAdd;
    }

    public void Add(Vector2 pos, float brushSize)
    {
        Add(pos, brushSize, destructionObjects[objectIndex]);
    }

    private void Add(Vector2 pos, float brushSize, DestructionObject destructionObject)
    {
        if(!canPlace) return;
        int randomCount = Random.Range(1, (int)(brushSize * 10));
        for (var i = 0; i < randomCount; i++)
        {
            Vector3 worldPos = new(pos.x, pos.y, 0);
            var offset = new Vector3(Random.Range(-brushSize * 5, brushSize * 5), Random.Range(-brushSize * 5, brushSize * 5), 0);
            Vector3 spawnPos = worldPos + offset;
            DestructionObject instancedDestructionObject = Instantiate(destructionObject, spawnPos, Quaternion.identity);
            instancedDestructionObject.Destroy(waterSystem, heightManager, spawnPos);
            canPlace = false;
        }
    }

    private void ResetAdd()
    {
        canPlace = true;
    }
    private void OnDisable()
    {
        TickSystem.ticked -= ResetAdd;
    }

    public void SetCurrentObject(int index)
    {
        objectIndex = index;
    }
}