using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SharkTornado : MonoBehaviour
{    
    private Vector3 destination;
    
    [SerializeField] private float maxStepDist;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private Vector3Int whereSharkTornadoSpawn; //Remove when have button

    private void Awake()
    {
        destination = transform.position;
        StartCoroutine(WaitForDestroy());
    }

    private IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (transform.position == destination)
        {
            destination.x = transform.position.x + Random.insideUnitCircle.x * maxStepDist;
            destination.z = transform.position.z + Random.insideUnitCircle.y * maxStepDist;
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

    public void SpawnGameObject() //Change when have button
    
    {   Vector3 spawnPoint = whereSharkTornadoSpawn;
        int topZ = TilemapManager.instance.GetColumnTopCoordinate(new Vector2Int(whereSharkTornadoSpawn.x, whereSharkTornadoSpawn.z));
        spawnPoint.y = topZ * 0.2f;
        
        Instantiate(gameObject, spawnPoint, Quaternion.identity);
    }
}
