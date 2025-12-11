using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DestructionComponent : MonoBehaviour
{
    [field:SerializeField] public List<DestructionObject> destructionObjects { get; private set; }
    //private HeightManager heightManager;
    public int objectIndex { get; private set; }
    private Vector2 lastPos;
    private Vector2 lastHitPos;
    private bool canPlace = true;

    private void OnEnable()
    {
        TickSystem.ticked += ResetAdd;
    }

    public void Destroy()
    {
        if(!canPlace) return;
        TilemapManager.instance.SpawnDestructionAtMouse(destructionObjects[objectIndex]);
        canPlace = false;
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