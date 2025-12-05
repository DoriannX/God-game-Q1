using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private Vector3 chunkSize;
    [SerializeField] private int chunkCount;

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
        public List<Vector3Int> tilesPosition = new List<Vector3Int>();
        public List<GameObject> tiles = new List<GameObject>();
        
    }
    
    public Vector2Int GetChunkIndexFromHex(Vector3Int hexCoord)
    {
        int chunkWidthHex = Mathf.RoundToInt(chunkSize.x / TilemapManager.instance.cellSize.x);
        int chunkHeightHex = Mathf.RoundToInt(chunkSize.z / TilemapManager.instance.cellSize.y);

        int cx = Mathf.FloorToInt((float)hexCoord.x / chunkWidthHex);
        int cy = Mathf.FloorToInt((float)hexCoord.y / chunkHeightHex);
        return new Vector2Int(cx, cy);
    }


    private void Start()
    { 
        SetChunk();
    }

    private void SetChunk()
    {
    }
    public void AddTileToChunk(Vector3Int hexCoord, GameObject tile)
    {
        Vector2Int chunkIndex = GetChunkIndexFromHex(hexCoord);

        if (!logicalChunks.TryGetValue(chunkIndex, out Chunk chunk))
        {
            chunk = new Chunk { index = chunkIndex };
            logicalChunks.Add(chunkIndex, chunk);
        }

        chunk.tilesPosition.Add(hexCoord);
        chunk.tiles.Add(tile);
    }

    public void SetInvisibleChunk(Vector3Int ChunkCoord)
    {
        Vector2Int targetIndex = new Vector2Int(0, 0);

        if (logicalChunks.TryGetValue(targetIndex, out Chunk chunk))
        {
            foreach (var tile in chunk.tiles)
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
            foreach (var tile in chunk.tiles)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetInvisibleChunk(Vector3Int.zero);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetVisibleChunk(Vector3Int.zero);
        }
    }
    // private void OnDrawGizmos()
    // {
    //     int r = (int)Mathf.Sqrt(chunkCount);
    //     if (r * r != chunkCount) return;
    //     Gizmos.color = Color.green;
    //
    //     Vector3 startPos = transform.position;
    //     Vector3 pos = startPos;
    //     for (int x = 0; x < r; x++)
    //     { 
    //         pos.x = startPos.x + chunkSize.x * x;
    //         for (int y = 0; y < r; y++)
    //         {
    //             
    //             pos.z = startPos.z + chunkSize.z * y;
    //
    //             
    //             Gizmos.DrawCube(pos, chunkSize);
    //             Gizmos.color = Color.white;
    //             Gizmos.DrawWireCube(pos, chunkSize);
    //             
    //             Gizmos.color = Color.green;
    //         }
    //
    //     }
    // }
}
