using UnityEngine;

public class WanderComponent : MonoBehaviour
{
    [SerializeField] private GhostIa ghostMovement;
    private bool canWander = true;

    public void Wander()
    {
        if (!ghostMovement.isMoving)
        {
            canWander = true;
        }

        if (!canWander) return;
        canWander = false;
        Debug.Log("Wandering");
        ghostMovement.GoByRandom();

    }
}