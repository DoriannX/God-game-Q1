using System;
using SaveLoadSystem;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    private Vector3 currentTarget;
    private Vector3 startPosition;

    private void Start()
    {
        currentTarget = transform.position;
        startPosition = transform.position;
    }

    public void GoTo(Vector3 position) {
        startPosition = transform.position;
        currentTarget = position;
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
