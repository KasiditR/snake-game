using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SnakeGame
{
    public class GridTile : MonoBehaviour
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Tile _grassPrefab;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _size;
        [SerializeField] private List<Tile> _tileGrids = new List<Tile>();

#if UNITY_EDITOR
        #region Editor Handle
        [ContextMenu("Generate")]
        private void Generate()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < this._width; x++)
                {
                    Tile obj = Instantiate(_grassPrefab, _parent);
                    obj.transform.position = new Vector3(x * _size, y * _size);
                    obj.SetPointTitle(new Vector2(x, y));
                    obj.name = $"({x},{y})";
                    _tileGrids.Add(obj);
                }
            }
        }

        [ContextMenu("Clear")]
        private void Clear()
        {
            foreach (var item in _tileGrids)
            {
                DestroyImmediate(item.gameObject);
            }
            _tileGrids.Clear();
        }
        #endregion
#endif

        public Tile GetRandomTile()
        {
            return _tileGrids[Random.Range(0, _tileGrids.Count)];
        }

        public bool HasTileInDirection(Transform player, Vector3 direction)
        {
            Vector3 nextPosition = player.position + (direction * 2);
            string tileName = $"({(int)(nextPosition.x / _size)},{(int)(nextPosition.y / _size)})";
            return _tileGrids.Exists(tile => tile.name == tileName);
        }

        public List<Tile> GetTilesInCenterArea()
        {
            List<Tile> tilesInArea = new List<Tile>();

            int minX = 5;
            int maxX = 24;
            int minY = 5;
            int maxY = 24;

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Tile tile = GetTileAtPosition(x, y);
                    if (tile != null)
                    {
                        tilesInArea.Add(tile);
                    }
                }
            }

            return tilesInArea;
        }

        public Tile GetTileAtPosition(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return null;
            }

            int index = y * _width + x;
            return _tileGrids[index];
        }
    }
}
