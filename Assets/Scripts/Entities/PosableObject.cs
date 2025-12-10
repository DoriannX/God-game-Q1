using UnityEngine;

public class PosableObject : Posable
{
    private House house;

    private void Awake()
    {
        house = GetComponent<House>();
    }
}