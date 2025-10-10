using System;
using System.Collections.Generic;
using UnityEngine;

public class WanderComponent : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private GhostIa ghostMovement;
    private bool canWander = true;
    private Vector2 targetPosition;

    public void Wander()
    {
        if (!ghostMovement.isMoving)
        {
            canWander = true;
        }
        
        if(!canWander) return;
        canWander = false;
        Vector2? randomPosition = ghostMovement.GetRandomWalkablePosition(wanderRadius);
        if(randomPosition == null) return;
        targetPosition = randomPosition.Value;
        ghostMovement.GoTo(randomPosition.Value);
        
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPosition, 0.1f);
    }
}