using UnityEngine;

public class Tornado : MeteoEvent
{
    [SerializeField] private float maxStepDist;
    [SerializeField] private float speed;
    private Vector3 destination;

    private void OnEnable()
    {
        TickSystem.ticked += OnTicked;
    }

    private void OnTicked()
    {
        if (transform.position == destination)
        {
            destination = Random.insideUnitSphere * maxStepDist;
        }
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    private void OnDisable()
    {
        TickSystem.ticked -= OnTicked;
    }
}
