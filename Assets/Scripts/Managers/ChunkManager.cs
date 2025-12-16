using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Vector3 chunkSize;
    [SerializeField] private int changeValueMinFromFov;
    [SerializeField] private int changeValueMaxFromFov;
    [SerializeField] private Camera mainCamera;

    public Dictionary<Vector2Int, Chunk> logicalChunks = new();

    [SerializeField] private int chunkViewRadius;
    public HashSet<Vector2Int> currentlyActiveChunks = new HashSet<Vector2Int>();

    public static ChunkManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

    public IEnumerator RemoveOneHeightForChunk(Vector2Int chunkIndex, float timeBetweenTileDestroy)
    {
        if (!logicalChunks.TryGetValue(chunkIndex, out Chunk chunk))
            yield break;

        List<GameObject> tiles = new List<GameObject>(chunk.gameObjectsInChunk);

        List<GameObject> tilesToRemove = new List<GameObject>();

        foreach (GameObject tile in tiles)
        {
            if (tile == null)
                continue;

            Vector3Int coord = TilemapManager.instance.WorldToHexAxial(tile.transform.position);

            int topZ = TilemapManager.instance.GetColumnTopCoordinate(new Vector2Int(coord.x, coord.y));

            if (coord.z == topZ)
                tilesToRemove.Add(tile);
        }

        foreach (GameObject tile in tilesToRemove)
        {
            Vector3Int coord = TilemapManager.instance.WorldToHexAxial(tile.transform.position);

            TilemapManager.instance.RemoveTileAt(coord);
            chunk.gameObjectsInChunk.Remove(tile);
            yield return new WaitForSeconds(timeBetweenTileDestroy);
        }
    }
}