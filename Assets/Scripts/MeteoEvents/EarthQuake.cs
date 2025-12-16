using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EarthQuake : MeteoEvent
{
    [SerializeField] private float timeBetweenTileDestroy;
    private void OnEnable()
    {
        RemoveOneHeightForTile();
    }

    private void RemoveOneHeightForTile()
    {
        if (ChunkManager.Instance.logicalChunks.Count == 0)
            return;

        Vector2Int randomIndex = GetRandomValidChunk();

        StartCoroutine(ChunkManager.Instance.RemoveOneHeightForChunk(randomIndex, timeBetweenTileDestroy));
    }

    private static readonly Vector2Int INVALID_CHUNK = new Vector2Int(int.MinValue, int.MinValue);

    private Vector2Int GetRandomValidChunk()
    {
        List<Vector2Int> validChunks = ChunkManager.Instance.logicalChunks
            .Where(kv => ChunkManager.Instance.currentlyActiveChunks.Contains(kv.Key))
            .Where(kv => kv.Value.gameObjectsInChunk.Count > 0)
            .Select(kv => kv.Key)
            .ToList();

        if (validChunks.Count == 0)
            return INVALID_CHUNK;

        return validChunks[Random.Range(0, validChunks.Count)];
    }
}