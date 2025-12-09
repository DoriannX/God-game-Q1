using System.Collections;
using System.Linq;
using UnityEngine;

public class EarthQuake : MeteoEvent
{
    private void OnEnable()
    {
        StartCoroutine(RemoveOneHeightForTile());
    }

    private IEnumerator RemoveOneHeightForTile()
    {
        // Si aucun chunk, on arrête
        if (ChunkManager.Instance.logicalChunks.Count == 0)
            yield break;

        // On récupère un chunk valide (actif + contient des tiles)
        Vector2Int randomIndex = GetRandomValidChunk();

        // Aucun chunk valide trouvé → on stoppe là
        if (randomIndex == INVALID_CHUNK)
        {
            Debug.LogWarning("Aucun chunk valide trouvé (actif + avec tiles)");
            yield break;
        }

        ChunkManager.Instance.RemoveOneHeightForChunk(randomIndex);
        yield return new WaitForSeconds(5);
        StartCoroutine(RemoveOneHeightForTile());
    }

    // Constante pour signaler un échec proprement
    private static readonly Vector2Int INVALID_CHUNK = new Vector2Int(int.MinValue, int.MinValue);

    private Vector2Int GetRandomValidChunk()
    {
        // On filtre : actifs + non vides
        var validChunks = ChunkManager.Instance.logicalChunks
            .Where(kv => ChunkManager.Instance.currentlyActiveChunks.Contains(kv.Key))
            .Where(kv => kv.Value.gameObjectsInChunk.Count > 0)
            .Select(kv => kv.Key)
            .ToList();

        if (validChunks.Count == 0)
            return INVALID_CHUNK;

        return validChunks[Random.Range(0, validChunks.Count)];
    }
}