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
        
        private Dictionary<TileBase, TileBase> _tileToPreviousHeight;
        private Dictionary<TileBase, int> _tileToHeightIndex;
        private int _totalHeightLevels;
        
        private void BuildCaches()
        {
            _tileToPreviousHeight = new Dictionary<TileBase, TileBase>(heightCells.Count);
            _tileToHeightIndex = new Dictionary<TileBase, int>(heightCells.Count);
            
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
        
        public TileBase GetUnderHeightTile(TileBase tile)
        {
            return _tileToPreviousHeight.TryGetValue(tile, out var previous) ? previous : null;
        }
        
        public TileBase GetUnderHeightTile(TileBase tile, int levels)
        {
            TileBase underTile = tile;
            for (int i = 0; i < levels; i++)
            {
                if (!_tileToPreviousHeight.TryGetValue(underTile, out var nextTile) || nextTile == null)
                    break;
                underTile = nextTile;
            }
            return underTile;
        }
        
        public int GetHeightIndex(TileBase tile)
        {
            return _tileToHeightIndex.TryGetValue(tile, out var index) ? index : 0;
        }
        
        private int CalculateHeightIndex(TileBase tile)
        {
            int index = 0;
            TileBase current = tile;
            
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