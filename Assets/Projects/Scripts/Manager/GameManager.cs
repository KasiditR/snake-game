using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnakeGame.Character;
using SnakeGame.Controller;
using SnakeGame.Interface;

namespace SnakeGame.Manager
{
    public sealed class GameManager : MonoBehaviour, IPausable
    {
        [SerializeField] private float _currentScore;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private AdventurerCharacter _adventurerPrefab;
        [SerializeField] private EnemyCharacter _enemyPrefab;
        [SerializeField] private GridTile _grid;

        [SerializeField] private float _adventureMaxCount;
        [SerializeField] private float _adventureCount;

        [SerializeField] private float _enemyMaxCount;
        [SerializeField] private float _enemyCount;

        private float _startTimer;
        private float _spawnTimer;
        private bool _isStartGame;
        private bool _isPause;
        private bool _isInit;
        private PlayerController _player;
        private List<EnemyCharacter> _enemyPools = new List<EnemyCharacter>();
        private List<AdventurerCharacter> _adventurerPools = new List<AdventurerCharacter>();

        public GridTile Grid { get => _grid; }

        private void Start()
        {
            StartCoroutine(InitializeRoutine());
        }
        private IEnumerator InitializeRoutine()
        {
            _isInit = false;
            yield return StartCoroutine(SpawnPlayerRoutine());
            yield return StartCoroutine(SpawnEnemyRoutine());
            yield return StartCoroutine(SpawnAdventurerRoutine());
            _startTimer = 4;
            _isInit = true;
        }

        private void Update()
        {
            if (_isPause || !_isInit)
            {
                return;
            }

            if (!_isStartGame)
            {
                StartGame();
                return;
            }

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0)
            {
                _spawnTimer = CoreGameManager.Instance.GameConfig.SpawnTimeInterval;
                StartCoroutine(SpawnEnemyRoutine());
            }
        }

        private void StartGame()
        {
            _startTimer -= Time.deltaTime;
            UIManager.Instance.UIGameplay.SetCountdownText(((int)_startTimer).ToString());
            if (_startTimer <= 0)
            {
                _isStartGame = true;
                _spawnTimer = CoreGameManager.Instance.GameConfig.SpawnTimeInterval;
                _player.Initialize(this);
                UIManager.Instance.UIGameplay.SetCountdownText(string.Empty);
            }
        }

        public void GameOver()
        {
            _isInit = false;
            UIManager.Instance.UIResult.Open((int)_currentScore);
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            while (_enemyCount < _enemyMaxCount)
            {
                var (position, isSpawn) = IsSpawn();
                if (isSpawn)
                {
                    _enemyCount++;
                    SpawnEnemy(position);
                }
                yield return null;
            }
        }

        private IEnumerator SpawnAdventurerRoutine()
        {
            while (_adventureCount < _adventureMaxCount)
            {
                var (position, isSpawn) = IsSpawn();
                if (isSpawn)
                {
                    _adventureCount++;
                    SpawnAdventurer(position);
                }
                yield return null;
            }
        }

        private IEnumerator SpawnPlayerRoutine()
        {
            List<Tile> tiles = _grid.GetTilesInCenterArea();
            while (_player == null)
            {
                Vector3 position = tiles[UnityEngine.Random.Range(0, tiles.Count)].GetTilePosition();
                Collider[] overlaps = Physics.OverlapSphere(position, 0.8f, LayerMask.GetMask(LayerSpawnOverlap()));
                if (overlaps.Length <= 0)
                {
                    _player = SpawnPlayer(position);
                }
                yield return null;
            }
        }

        private EnemyCharacter SpawnEnemy(Vector2 position)
        {
            EnemyCharacter enemy = _enemyPools.Find(x => x.IsDie);
            if (enemy == null)
            {
                enemy = Instantiate(_enemyPrefab, this.transform);
                _enemyPools.Add(enemy);
            }

            enemy.Initialize();
            enemy.OnDie += OnEnemyDie;
            enemy.transform.position = position;
            return enemy;
        }

        private AdventurerCharacter SpawnAdventurer(Vector2 position)
        {
            AdventurerCharacter adventurer = _adventurerPools.Find(x => x.IsDie);
            if (adventurer == null)
            {
                adventurer = Instantiate(_adventurerPrefab, this.transform);
                _adventurerPools.Add(adventurer);
            }

            adventurer.Initialize();
            adventurer.OnDie += OnAdventurerDie;
            adventurer.transform.position = position;
            return adventurer;
        }

        private PlayerController SpawnPlayer(Vector2 position)
        {
            PlayerController player = Instantiate(_playerController);
            player.transform.position = position;
            AdventurerCharacter adventurer = SpawnAdventurer(position);
            _adventureCount++;
            player.AddMemberParty(adventurer);
            adventurer.SetLeader();
            return player;
        }

        private void OnAdventurerDie(BaseCharacter character)
        {
            _adventureCount--;
        }

        private void OnEnemyDie(BaseCharacter character)
        {
            _enemyCount--;
            EnemyCharacter enemy = character as EnemyCharacter;
            _currentScore += enemy.ScoreDrop;
            UIManager.Instance.UIGameplay.SetScoreText(_currentScore.ToString());
            enemy.OnDie -= OnEnemyDie;
        }

        private (Vector3, bool) IsSpawn()
        {
            Tile tile = _grid.GetRandomTile();
            Collider[] overlaps = Physics.OverlapSphere(tile.GetTilePosition(), 0.8f, LayerMask.GetMask(LayerSpawnOverlap()));
            if (overlaps.Length <= 0)
            {
                return (tile.GetTilePosition(), true);
            }
            else
            {
                return (Vector3.zero, false);
            }
        }

        private string[] LayerSpawnOverlap()
        {
            return new string[] { LayerName.PLAYER, LayerName.ENEMY, LayerName.ADVENTURER };
        }

        public void OnPause()
        {
            _isPause = true;
        }

        public void OnResume()
        {
            _isPause = false;
        }
    }
}