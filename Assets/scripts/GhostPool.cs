using UnityEngine;
using UnityEngine.Pool;

public class GhostPool : MonoBehaviour
{
    [SerializeField] private GhostIa ghostPrefab;
    private IObjectPool<GhostIa> m_ghostPool;
    private IObjectPool<GhostIa> ghostPool
    {
        get
        {
            if (m_ghostPool == null)
            {
                m_ghostPool = new ObjectPool<GhostIa>(CreateGhost, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 10000, 20000);
            }
            return m_ghostPool;
        }
    }

    private GhostIa CreateGhost()
    {
        GhostIa ghost = Instantiate(ghostPrefab);
        return ghost;
    }
    
    private void OnReturnedToPool(GhostIa ghost)
    {
        ghost.gameObject.SetActive(false);
    }
    
    private void OnTakeFromPool(GhostIa ghost)
    {
        ghost.gameObject.SetActive(true);
    }
    
    private void OnDestroyPoolObject(GhostIa ghost)
    {
        Destroy(ghost.gameObject);
    }
}
