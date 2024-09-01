using UnityEngine;

namespace SnakeGame.Manager
{
    public sealed class CoreGameManager : Singleton<CoreGameManager>
    {
        [SerializeField] private GameConfigSO _gameConfig;

        public GameConfigSO GameConfig { get => _gameConfig; }
    }
}
