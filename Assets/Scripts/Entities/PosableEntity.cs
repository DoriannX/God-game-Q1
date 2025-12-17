using UnityEngine;

public class PosableEntity : Posable
{
    public EntityType GetEntityType()  => GetComponent<EntityAI>().GetEntityType();
}

