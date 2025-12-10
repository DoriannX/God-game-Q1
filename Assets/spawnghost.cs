using UnityEngine;

public class spawnghost : MonoBehaviour
{
    
    //TODO: REMOVE
    public GameObject ghost;
    public void SpawnGhost() {
        Instantiate(ghost, transform.position, transform.rotation);
    }
}
