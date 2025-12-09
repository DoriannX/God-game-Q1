using UnityEngine;

public class spawnghost : MonoBehaviour
{
    public GameObject ghost;
    public void SpawnGhost() {
        Instantiate(ghost, transform.position, transform.rotation);
    }
}
