
using System.Collections.Generic;
using UnityEngine;

public class Tsunami : MeteoEvent
{
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private int puddleSize;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float lifeTime;
    [SerializeField] private float speed;
    private Vector3Int[] brush;
    private Vector3 direction;
    private float timer = 0;


    private void OnEnable()
    {
        //Spawn outside the camera view
        PlaceAtCameraCenter();
        Vector2 forgor = new Vector2(GetFOVWorldWidth(), GetFOVWorldHeight());
        float hypothenus = Mathf.Sqrt(Mathf.Pow(forgor.x, 2) + Mathf.Pow(forgor.y, 2));
        Vector3 hypothenusVector = new Vector3(hypothenus, hypothenus);
        Vector3 spawnPoint = Random.insideUnitCircle * forgor;
        if (spawnPoint.x < 0)
        {
            hypothenusVector.x *= -1;
        }
        if (spawnPoint.y < 0)
        {
            hypothenusVector.y *= -1;
        }
        spawnPoint += hypothenusVector;
        float temp = spawnPoint.y;
        spawnPoint.y = spawnPoint.z;
        spawnPoint.z = temp;
        direction = transform.position - spawnPoint;
        transform.position = spawnPoint;

        brush = GetBrushArea(TilemapManager.instance.WorldToHexAxial(transform.position));
        foreach (Vector3Int tileInd in brush)
        {
            TilemapManager.instance.SpawnTileAt(tileInd, waterPrefab);
        }
    }

    private void Update()
    {
        if (timer >= lifeTime)
        {
            timer = 0;
            gameObject.SetActive(false);
            return;
        }
        //transform.Translate(direction * speed * Time.deltaTime);
        transform.position += direction * speed * Time.deltaTime;
        brush = GetBrushArea(TilemapManager.instance.WorldToHexAxial(transform.position));
        foreach (Vector3Int tileInd in brush)
        {
            if (TilemapManager.instance.GetTile(tileInd) == null)
            {
                TilemapManager.instance.SpawnTileAt(tileInd, waterPrefab);
                continue;
            }
            Vector3Int toptTile = new Vector3Int(tileInd.x, tileInd.y, TilemapManager.instance.GetColumnTopCoordinate((Vector2Int)tileInd));
            TilemapManager.instance.SpawnTileAt(toptTile, waterPrefab);
        }
    }

    private Vector3Int[] GetBrushArea(Vector3Int centerHex)
    {
        if (puddleSize == 1)
        {
            return new Vector3Int[] { new(centerHex.x, centerHex.y, 0) };
        }

        int radius = puddleSize - 1;
        List<Vector3Int> hexagons = new();

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                hexagons.Add(new Vector3Int(centerHex.x + q, centerHex.y + r));
            }
        }

        return hexagons.ToArray();
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

    private float GetFOVWorldHeight()
    {
        Vector2 screenCenter;
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 point1 = ray.GetPoint(distance);
            Vector2 screenTop = new Vector2(Screen.width / 2, Screen.height);
            ray = mainCamera.ScreenPointToRay(screenTop);
            if (groundPlane.Raycast(ray, out distance))
            {
                Vector3 point2 = ray.GetPoint(distance);
                return Vector3.Distance(point1, point2);
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    private float GetFOVWorldWidth()
    {
        Vector2 screenCenter;
        screenCenter.x = Screen.width / 2;
        screenCenter.y = Screen.height / 2;
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            Vector3 point1 = ray.GetPoint(distance);
            Vector2 screenRight = new Vector2(Screen.width, Screen.height / 2);
            ray = mainCamera.ScreenPointToRay(screenRight);
            if (groundPlane.Raycast(ray, out distance))
            {
                Vector3 point2 = ray.GetPoint(distance);
                return Vector3.Distance(point1, point2);
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
}