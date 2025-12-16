using UnityEngine;

public class PosableEntity : Posable
{
    [HideInInspector] public EntityType entityType;

    private void Awake()
    {
        entityType = GetComponent<EntityIA>().entityType;
    }
}

