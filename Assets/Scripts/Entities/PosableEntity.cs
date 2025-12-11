using UnityEngine;

public class PosableEntity : Posable
{
    private GhostIa ghostIa;

    private void Awake()
    {
        ghostIa = GetComponent<GhostIa>();
    }
}

