using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(ScareComponent))]
public class WaterChecker : MonoBehaviour
{
    [SerializeField] private float checkRadius = 5f;
    private ScareComponent scareComponent;

    private void Awake()
    {
        scareComponent = GetComponent<ScareComponent>();
    }

    private void OnEnable()
    {
        TilemapManager.instance.onWaterCellChanged += CheckWater;
    }

    private void CheckWater(Vector3Int obj)
    {
        Vector2 tilePos = TilemapManager.instance.tilemap.CellToWorld(obj);
        if(Vector2.Distance(transform.position, tilePos) < checkRadius)
        {
            scareComponent.Scare(tilePos);
        }
    }
    
    private void OnDisable()
    {
        TilemapManager.instance.onWaterCellChanged -= CheckWater;
    }
}
