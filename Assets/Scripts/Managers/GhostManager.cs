using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager instance { get; private set; }
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private int maxGhosts = 100;
    HashSet<GameObject> ghosts = new();
    public event Action<int> onGhostsChanged;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public GameObject SpawnGhost(Vector3 position)
    {
        if (ghosts.Count >= maxGhosts)
        {
            Debug.LogWarning("Max ghosts reached");
            return null;
        }
        GameObject ghost = Instantiate(ghostPrefab, position, Quaternion.identity);
        ghosts.Add(ghost);
        onGhostsChanged?.Invoke(ghosts.Count);
        return ghost;
    }

    public void RemoveGhost(GameObject ghost)
    {
        if (ghosts.Remove(ghost))
        {
            Destroy(ghost);
            onGhostsChanged?.Invoke(ghosts.Count);
        }
    }

    public void RegisterGhost(GhostIa ghostIa)
    {
        ghosts.Add(ghostIa.gameObject);
        onGhostsChanged?.Invoke(ghosts.Count);
    }

    public void UnregisterGhostInHouse(House house)
    {
        for (int numberOfGhostInHouse = 0; numberOfGhostInHouse < house.fuckingGhosts.Count; numberOfGhostInHouse++)
        {
            ghosts.Remove(house.fuckingGhosts.ElementAt(numberOfGhostInHouse).gameObject);
        }
        Destroy(house.gameObject);
        onGhostsChanged?.Invoke(ghosts.Count);
    }
}