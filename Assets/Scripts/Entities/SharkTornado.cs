using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SharkTornado : MonoBehaviour
{    
    [SerializeField] private float maxStepDist;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    private Vector3 destination;

    private void Start()
    {
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

    public void SpawnGameObject()
    {
        Instantiate(gameObject, transform.position, Quaternion.identity);
    }
}
