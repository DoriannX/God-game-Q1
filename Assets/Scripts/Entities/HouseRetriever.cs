using UnityEngine;

public class HouseRetriever : MonoBehaviour
{
    private readonly Collider2D[] colliders = new Collider2D[100];
    [SerializeField] private float range;
    public House GetNearestHouse(Vector2 position)
    {
        House nearestHouse = null;
        var nearestDistance = float.MaxValue;
        
        var filter = new ContactFilter2D();
        
        int count = Physics2D.OverlapCircle(position, range, filter, colliders);
        for (var i = 0; i < count; i++)
        {
            var house = colliders[i].GetComponent<House>();
            if (house == null)
            {
                continue;
            }

            if (house.isFull)
            {
                continue;
            }
            float distance = Vector2.Distance(position, house.transform.position);
            if (distance >= nearestDistance)
            {
                continue;
            }
            nearestDistance = distance;
            nearestHouse = house;
        }

        return nearestHouse;
    }
}
