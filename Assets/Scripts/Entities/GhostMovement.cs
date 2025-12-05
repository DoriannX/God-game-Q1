using System;
using SaveLoadSystem;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    private Vector3 currentTarget;
    private Vector3 startPosition;
    private float targetHeight;

    private void Start()
    {
        currentTarget = transform.position;
        startPosition = transform.position;
        targetHeight = transform.position.z;
    }

    public void GoTo(Vector2 position, int newHeight)
    {
        startPosition = transform.position;
        currentTarget = new Vector3(position.x, position.y, newHeight);
        targetHeight = newHeight;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        float time = TickSystem.instance.tickInterval;
        float distance = Vector3.Distance(startPosition, currentTarget);
        float speed = distance / time;
        transform.position = Vector3.MoveTowards(currentPosition, currentTarget, speed * Time.deltaTime);
    }

    public bool isMoving =>
        Vector3.Distance(transform.position, currentTarget) > 0.01f;
}
