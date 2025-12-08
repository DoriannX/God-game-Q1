using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Vector3 chunkSize;
    [SerializeField] private Camera mainCamera;

    public Dictionary<Vector2Int, Chunk> logicalChunks = new();
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

    public void SetVisibilityOfChunk()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach (var kvp in logicalChunks)
        {
            Chunk chunk = kvp.Value;

            bool isVisible = GeometryUtility.TestPlanesAABB(planes, chunk.bounds);

            foreach (GameObject tile in chunk.gameObjectsInChunk)
            {
                if (tile != null)
                    tile.SetActive(isVisible);
            }
        }
    }

    private void Update()
    {
        SetVisibilityOfChunk();
    }
}
