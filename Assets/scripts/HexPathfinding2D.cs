using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexPathfinding2D : MonoBehaviour
{
    public Tilemap tilemap;
    //Water tilemap
    public TileBase[] walkableTiles;
    [SerializeField] private HeightManager heightManager;

    static readonly Vector2Int[] evenRowNeighbors = new Vector2Int[]
    {
        new Vector2Int(+1, 0),
        new Vector2Int(0, +1),
        new Vector2Int(-1, +1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
    };

    static readonly Vector2Int[] oddRowNeighbors = new Vector2Int[]
    {
        new Vector2Int(+1, 0),
        new Vector2Int(+1, +1),
        new Vector2Int(0, +1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(+1, -1),
    };
    
    public List<Vector2> GetWorldPath(List<Vector3Int> path)
    {
        if (path == null) return null;

        var worldPath = new List<Vector2>();
        foreach (var cell in path)
        {
            worldPath.Add(tilemap.GetCellCenterWorld(cell));
        }
        return worldPath;
    }
    
    public List<Vector3Int> FindPath(Vector2 startWorld, Vector2 goalWorld)
    {
        Vector3Int start = tilemap.WorldToCell(startWorld);
        Vector3Int goal = tilemap.WorldToCell(goalWorld);
        return FindPath(start, goal);
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        var openSet = new PriorityQueue<Vector3Int>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();
            if (current == goal)
                return Reconstruct(cameFrom, current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!IsWalkable(neighbor) || !IsHeightWalkable(current, neighbor))
                {
                    continue;
                }

                float tentative = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentative < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative;
                    float f = tentative + Heuristic(neighbor, goal);
                    openSet.Enqueue(neighbor, f);
                }
            }
        }
        return null;
    }

    List<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        var list = new List<Vector3Int>();
        var rowParity = pos.y & 1;
        var offsets = rowParity == 0 ? evenRowNeighbors : oddRowNeighbors;

        foreach (var o in offsets)
        {
            var neighbor = new Vector3Int(pos.x + o.x, pos.y + o.y, 0);
            list.Add(neighbor);
        }
        return list;
    }

    bool IsWalkable(Vector3Int pos)
    {
        var tile = tilemap.GetTile(pos);
        return tile != null && walkableTiles.Contains(tile);
    }
    
    private bool IsHeightWalkable(Vector3Int from, Vector3Int to)
    {
        int fromHeight = heightManager.GetHeightIndex(tilemap.GetTile(from));
        int toHeight = heightManager.GetHeightIndex(tilemap.GetTile(to));
        return Mathf.Abs(fromHeight - toHeight) <= 1;
    }

    float Heuristic(Vector3Int a, Vector3Int b)
    {
        // distance hex approx
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        return Mathf.Abs(dx) + Mathf.Abs(dy);
    }

    List<Vector3Int> Reconstruct(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
}
