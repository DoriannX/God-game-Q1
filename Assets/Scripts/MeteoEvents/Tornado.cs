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
            destination.x = transform.position.x + Random.insideUnitCircle.x * maxStepDist;
            destination.z = transform.position.z + Random.insideUnitCircle.y * maxStepDist;
            Vector3Int destinationPoint = TilemapManager.instance.WorldToHexAxial(destination);
            int topZ = TilemapManager.instance.GetColumnTopCoordinate(new Vector2Int(destinationPoint.x, destinationPoint.y));
            destination = TilemapManager.instance.HexAxialToWorld(destinationPoint);
            destination.y = topZ * 0.2f + 0.8f;
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