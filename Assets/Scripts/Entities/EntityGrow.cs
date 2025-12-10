using UnityEngine;

public class EntityGrow : MonoBehaviour {
    [SerializeField] GrowComponent growComponent;
    
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