using UnityEngine;

public class SandStorm : MeteoEvent
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject sandPrefab;
    [SerializeField] private float sandTickInterval;
    private Vector3 destination;
    private float timer = 0;

    private void OnEnable()
    {
        PlaceAtCameraCenter();
    }

    private void Update()
    {
        if (timer >= sandTickInterval)
        {
            PlaceAtCameraCenter();

            destination = Random.insideUnitCircle * mainCamera.fieldOfView;
            float temp = destination.y;
            destination.y = 0;
            destination.z = temp;
            timer = 0;
        }
        Vector3Int hexDest = TilemapManager.instance.WorldToHexAxial(destination);
        TilemapManager.instance.SpawnTileAt(hexDest, sandPrefab);
        timer += Time.deltaTime;
    }

    private void PlaceAtCameraCenter()
    {
        Vector2 screenCenter;
        screenCenter.x = Screen.width/2;
        screenCenter.y = Screen.height/2;
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
