using UnityEngine;

public class PosableEntity : Posable
{
    public EntityType entityType;

    private void Awake()
    {
        entityType = GetComponent<EntityAI>().entityType;
    }
}

