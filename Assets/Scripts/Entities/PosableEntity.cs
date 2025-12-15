using UnityEngine;

public class PosableEntity : Posable
{
    private EntityIA entityIa;

    private void Awake()
    {
        entityIa = GetComponent<EntityIA>();
    }
}

