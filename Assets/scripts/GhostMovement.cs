using System;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    private Vector2 currentTarget;
    private Vector2 startPosition;

    private void Start()
    {
        currentTarget = transform.position;
    }

    public void GoTo(Vector2 position)
    {
        startPosition = transform.position;
        currentTarget = position;
    }

    private void Update()
    {
        Vector2 currentPosition = transform.position;
        float time = TickSystem.instance.tickInterval;
        float distance = Vector2.Distance(startPosition, currentTarget);
        float speed = distance / time;
        transform.position = Vector2.MoveTowards(currentPosition, currentTarget, 2f * speed * Time.deltaTime);
    }

    public bool isMoving =>
        Vector2.Distance(transform.position, currentTarget) > 0.01f;
}