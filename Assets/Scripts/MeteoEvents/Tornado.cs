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
        PlaceAtCameraCenter();
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
            EntityManager.instance.RemoveEntitiesInHouse(houseHit);
            Destroy(houseHit.gameObject);
        }
    }

    private void PlaceAtCameraCenter()
    {
        Vector2 screenCenter;
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
        }
        else
        {
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float distance;

            if (groundPlane.Raycast(ray, out distance))
            {
                transform.position = ray.GetPoint(distance);
            }
            else
            {
                Debug.LogWarning("Could not calculate world position");
                return;
            }
        }
    }
}