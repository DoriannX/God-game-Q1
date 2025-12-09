using UnityEngine;

public class Tornado : MeteoEvent
{
    [SerializeField] private float maxStepDist;
    [SerializeField] private float speed;
    [SerializeField] private Camera mainCamera;
    private Vector3 destination;

    private void OnEnable()
    {
        //TickSystem.ticked += OnTicked;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
        }
    }

    private void Update()
    {
        if (transform.position == destination)
        {
            destination = Random.insideUnitCircle * maxStepDist;
            float temp = destination.y;
            destination.y = 0;
            destination.z = temp;
        }
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        House houseHit = collision.gameObject.GetComponent<House>();
        if (houseHit != null)
        {
            GhostManager.instance.UnregisterGhostInHouse(houseHit);
            Destroy(houseHit.gameObject);
        }
    }

    /*private void OnDisable()
    {
        TickSystem.ticked -= OnTicked;
    }*/
}
