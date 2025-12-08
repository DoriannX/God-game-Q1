using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Vector3 chunkSize;
    [SerializeField] private Camera mainCamera;

    public Dictionary<Vector2Int, Chunk> logicalChunks = new();
    
    [SerializeField] private int chunkViewRadius;  
    private HashSet<Vector2Int> currentlyActiveChunks = new HashSet<Vector2Int>();

    public static ChunkManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    [Serializable]
    public class Chunk
    {
        public Vector2Int index;
        public List<GameObject> gameObjectsInChunk = new List<GameObject>();
        public Bounds bounds;
    }

    private Vector2Int GetChunkIndexFromWorld(Vector3 worldPos)
    {
        int cx = Mathf.FloorToInt(worldPos.x / chunkSize.x);
        int cy = Mathf.FloorToInt(worldPos.z / chunkSize.z);

        return new Vector2Int(cx, cy);
    }
    
    public void AddGameObjectToChunk(Vector3 worldPos, GameObject gameObjectToAdd)
    {
        Vector2Int chunkIndex = GetChunkIndexFromWorld(worldPos);
        
        if (!logicalChunks.TryGetValue(chunkIndex, out Chunk chunk))
        {
            Vector3 chunkCenter = new Vector3(
                chunkIndex.x * chunkSize.x + chunkSize.x / 2f,
                0,
                chunkIndex.y * chunkSize.z + chunkSize.z / 2f
            );

            chunk = new Chunk
            {
                index = chunkIndex,
                bounds = new Bounds(chunkCenter, chunkSize)
            };

            logicalChunks.Add(chunkIndex, chunk);
        }

        chunk.gameObjectsInChunk.Add(gameObjectToAdd);
    }

    public void UpdateVisibleChunks()
    {
        Vector2Int cameraChunk = GetChunkIndexFromWorld(mainCamera.transform.position);
        
        HashSet<Vector2Int> newActiveChunks = new HashSet<Vector2Int>();
        Vector3 test = mainCamera.fieldOfView * chunkSize;
        Debug.Log(test);
        for (int dx = -chunkViewRadius; dx <= chunkViewRadius; dx++)
        {
            for (int dy = -chunkViewRadius; dy <= chunkViewRadius; dy++)
            {
                Vector2Int index = new Vector2Int(cameraChunk.x + dx, cameraChunk.y + dy);

                newActiveChunks.Add(index);

                if (logicalChunks.TryGetValue(index, out Chunk chunk))
                {
                    if (!currentlyActiveChunks.Contains(index))
                        SetChunkActive(chunk, true);
                }
            }
        }
        
        foreach (Vector2Int old in currentlyActiveChunks)
        {
            if (!newActiveChunks.Contains(old))
            {
                if (logicalChunks.TryGetValue(old, out Chunk chunk))
                    SetChunkActive(chunk, false);
            }
        }
        
        currentlyActiveChunks = newActiveChunks;
    }


    
    private void SetChunkActive(Chunk chunk, bool active)
    {
        foreach (GameObject tile in chunk.gameObjectsInChunk)
        {
            if (tile != null)
                tile.SetActive(active);
        }
    }


    
    public void SetInvisibleChunk(Vector3Int ChunkCoord)
    {
        Vector2Int targetIndex = new Vector2Int(0, 0);

        if (logicalChunks.TryGetValue(targetIndex, out Chunk chunk))
        {
            foreach (var tile in chunk.gameObjectsInChunk)
            {
                if (tile == null)
                {
                    Debug.Log("Tile déjà détruit !");
                    continue;
                }

                Debug.Log("Tile OK");
                tile.SetActive(false);
            }
        }
    }
    
    public void SetVisibleChunk(Vector3Int ChunkCoord)
    {
        Vector2Int targetIndex = new Vector2Int(0, 0);

        if (logicalChunks.TryGetValue(targetIndex, out Chunk chunk))
        {
            foreach (var tile in chunk.gameObjectsInChunk)
            {
                if (tile == null)
                {
                    Debug.Log("Tile déjà détruit !");
                    continue;
                }

                Debug.Log("Tile OK");
                tile.SetActive(true);
            }
        }
    }

    private void Update()
    {
        UpdateVisibleChunks();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetInvisibleChunk(Vector3Int.zero);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetVisibleChunk(Vector3Int.zero);
        }
    }
}
