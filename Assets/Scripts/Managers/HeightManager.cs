using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;
    
    public class HeightManager : MonoBehaviour
    {
        public static HeightManager instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                BuildCaches();
            }
            else
                Destroy(gameObject);
        }
        
        [SerializeField] private List<HeightCell> heightCells;
        
        private Dictionary<GameObject, GameObject> _tileToPreviousHeight;
        private Dictionary<GameObject, int> _tileToHeightIndex;
        private int _totalHeightLevels;
        
        private void BuildCaches()
        {
            _tileToPreviousHeight = new Dictionary<GameObject, GameObject>(heightCells.Count);
            _tileToHeightIndex = new Dictionary<GameObject, int>(heightCells.Count);
            
            foreach (var cell in heightCells)
            {
                _tileToPreviousHeight[cell.tile] = cell.previousHeightTile;
            }
            
            foreach (var cell in heightCells)
            {
                if (!_tileToHeightIndex.ContainsKey(cell.tile))
                {
                    _tileToHeightIndex[cell.tile] = CalculateHeightIndex(cell.tile);
                }
            }
            
            _totalHeightLevels = CalculateTotalHeightLevels();
        }
        
        public GameObject GetUnderHeightTile(GameObject tile)
        {
            return _tileToPreviousHeight.TryGetValue(tile, out var previous) ? previous : null;
        }
        
        public GameObject GetUnderHeightTile(GameObject tile, int levels)
        {
            GameObject underTile = tile;
            for (int i = 0; i < levels; i++)
            {
                if (!_tileToPreviousHeight.TryGetValue(underTile, out var nextTile) || nextTile == null)
                    break;
                underTile = nextTile;
            }
            return underTile;
        }
        
        public int GetHeightIndex(GameObject tile)
        {
            return _tileToHeightIndex.TryGetValue(tile, out var index) ? index : 0;
        }
        
        private int CalculateHeightIndex(GameObject tile)
        {
            int index = 0;
            GameObject current = tile;
            
            while (_tileToPreviousHeight.TryGetValue(current, out var previous) && previous != null)
            {
                index++;
                current = previous;
            }
            
            return index;
        }
        
        public int GetTotalHeightLevels()
        {
            return _totalHeightLevels;
        }
        
        private int CalculateTotalHeightLevels()
        {
            int maxIndex = 0;
            foreach (var kvp in _tileToHeightIndex)
            {
                if (kvp.Value > maxIndex)
                    maxIndex = kvp.Value;
            }
            return maxIndex + 1;
        }
    }