using UnityEngine;

public class WanderComponent : MonoBehaviour
{
    [SerializeField] private EntityAI entityAI;
    private bool canWander = true;

    public void Wander() {
        if (!entityAI.isMoving) {
            canWander = true;
        }

        if (!canWander) {
            return;
        }
        canWander = false;
        entityAI.GoByRandom();
    }
}