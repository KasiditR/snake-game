using UnityEngine;

namespace SnakeGame
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Vector2 _pointTitle;

        public SpriteRenderer SpriteRenderer { get => _spriteRenderer; }


        public Vector2 GetTilePosition()
        {
            return this.transform.position;
        }

        public Vector2 GetPointTitle()
        {
            return _pointTitle;
        }

        public void SetPointTitle(Vector2 value)
        {
            _pointTitle = value;
        }
    }
}
