using UnityEngine;

namespace SnakeGame.Manager
{
    public sealed class CoreGameManager : Singleton<CoreGameManager>
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private GameConfigSO _gameConfig;

        public GameConfigSO GameConfig { get => _gameConfig; }
        public GameManager GameManager { get => _gameManager; set => _gameManager = value; }
    }
}
