using System;
using SaveLoadSystem;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    private Vector2 currentTarget;
    private Vector2 startPosition;
    private int height;

    private void Start()
    {
        currentTarget = transform.position;
    }

    public void GoTo(Vector2 position, int newHeight)
    {
        startPosition = transform.position;
        currentTarget = position;
        height = newHeight;
    }

    private void Update()
    {
        Vector2 currentPosition = transform.position;
        float time = TickSystem.instance.tickInterval;
        float distance = Vector2.Distance(startPosition, currentTarget);
        float speed = distance / time;
        transform.position = Vector2.MoveTowards(currentPosition, currentTarget, speed * Time.deltaTime);
        Vector3 newPos = transform.position;
        newPos.z = height;
        transform.position = newPos;
    }

    public bool isMoving =>
        Vector2.Distance(transform.position, currentTarget) > 0.01f;
}