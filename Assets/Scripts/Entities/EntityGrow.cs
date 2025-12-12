using UnityEngine;

public class EntityGrow : MonoBehaviour {
    [SerializeField] GrowComponent growComponent;
    
    public bool isFullyGrown;
    
    private void Awake() {
        growComponent.onFullyGrown += () => isFullyGrown = true;
    }
    
    private void OnEnable() {
        TickSystem.ticked += Tick;
    }
    
    private void OnDisable() {
        TickSystem.ticked -= Tick;
    }
    
    private void Tick() {
        growComponent.Grow();
    }
}