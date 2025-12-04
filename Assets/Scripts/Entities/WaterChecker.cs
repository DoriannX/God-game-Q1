using UnityEngine;

/// <summary>
/// TEMPORARILY DISABLED - Water checking disabled for new hexagonal TilemapManager.
/// Component preserved for future re-implementation.
/// </summary>
[RequireComponent(typeof(ScareComponent))]
public class WaterChecker : MonoBehaviour
{
    [Header("WATER CHECKING DISABLED")]
    [SerializeField] private float checkRadius = 5f;
    private ScareComponent scareComponent;

    private void Awake()
    {
        scareComponent = GetComponent<ScareComponent>();
        Debug.LogWarning("WaterChecker is DISABLED - Water system not available with new TilemapManager");
    }

    private void OnEnable()
    {
        // DISABLED: Water event subscription
        // TilemapManager.instance.onWaterCellChanged += CheckWater;
    }

    private void CheckWater(Vector3Int obj)
    {
        // DISABLED: Water checking
    }
    
    private void OnDisable()
    {
        // DISABLED: Water event unsubscription
        // TilemapManager.instance.onWaterCellChanged -= CheckWater;
    }
}
