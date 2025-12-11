using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Vector3 chunkSize;
    [SerializeField] private int changeValueMinFromFov;
    [SerializeField] private int changeValueMaxFromFov;
    [SerializeField] private Camera mainCamera;

    private Dictionary<Vector2Int, Chunk> logicalChunks = new();

    [SerializeField] private int chunkViewRadius;
    private HashSet<Vector2Int> currentlyActiveChunks = new();

    private void OnEnable()
    {
        if (ChunkManager.Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        TilemapManager.instance.tilePlaced += OnTilePlaced;
        TilemapManager.instance.tileRemoved += OnTileRemoved;
    }

    private void OnTileRemoved(Vector3Int coord)
    {
        RemoveGameObjectFromChunk(TilemapManager.instance.HexAxialToWorld(coord),
            TilemapManager.instance.GetTile(coord));
    }

    private void OnTilePlaced(Vector3Int coord)
    {
        AddGameObjectToChunk(TilemapManager.instance.HexAxialToWorld(coord), TilemapManager.instance.GetTile(coord));
    }

    [Serializable]
    public class Chunk
    {
        public Vector2Int index;
        public List<GameObject> gameObjectsInChunk = new();
        public Bounds bounds;
    }

    private Vector2Int GetChunkIndexFromWorld(Vector3 worldPos)
    {
        int cx = Mathf.FloorToInt(worldPos.x / chunkSize.x);
        int cy = Mathf.FloorToInt(worldPos.z / chunkSize.z);

        return new Vector2Int(cx, cy);
    }

    private void RemoveGameObjectFromChunk(Vector3 worldPos, GameObject gameObjectToRemove)
    {
        Vector2Int chunkIndex = GetChunkIndexFromWorld(worldPos);

        if (logicalChunks.TryGetValue(chunkIndex, out Chunk chunk))
        {
            chunk.gameObjectsInChunk.Remove(gameObjectToRemove);
        }
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

    private void UpdateVisibleChunks()
    {
        if (mainCamera.fieldOfView >= changeValueMaxFromFov) chunkViewRadius = 3; //Change if the zoom is not with FOV
        else if (mainCamera.fieldOfView <= changeValueMinFromFov) chunkViewRadius = 1;
        else chunkViewRadius = 2;

        Vector2Int cameraChunk = GetChunkIndexFromWorld(mainCamera.transform.position);

        HashSet<Vector2Int> newActiveChunks = new HashSet<Vector2Int>();
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

    private void Update()
    {
        UpdateVisibleChunks(); //Need to remove this and put it in the script of the movement and the zoom of the camera
    }
}