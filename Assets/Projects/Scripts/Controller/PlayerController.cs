using UnityEngine;
using Random = UnityEngine.Random;
using SnakeGame.Manager;
using SnakeGame.Interface;

namespace SnakeGame.Controller
{
    public class PlayerController : MonoBehaviour, IPausable
    {
        [SerializeField] private float _timeToMove;
        [SerializeField] private PlayerCollisionHandler _playerCollisionHandler;
        [SerializeField] private PartyHandler _partyHandler;
        private float _lastDirectionChangeTime;
        private float _moveElapsedTime;
        private bool _isMoving;
        private bool _isInit;
        private bool _isPause;
        private GridTile _grid;
        private GameManager _gameManager;
        private Vector3 _origPos;
        private Vector3 _targetPos;
        private Vector3 _direction;

        public float TimeToMove { get => _timeToMove; set => _timeToMove = value; }
        public PartyHandler PartyHandler { get => _partyHandler; }
        public bool IsPause { get => _isPause; set => _isPause = value; }

        private void Start()
        {
            _partyHandler.Initialize(this);
            _playerCollisionHandler.Initialize(this, _partyHandler);
        }

        public void Initialize(GameManager gameManager)
        {
            this._gameManager = gameManager;
            _grid = _gameManager.Grid;
            _direction = Vector3.down;
            _isInit = true;
        }

        private void OnEnable()
        {
            PauseManager.Instance.RegisterPausable(this);
        }

        private void OnDisable()
        {
            PauseManager.Instance.UnregisterPausable(this);
        }

        private void Update()
        {
            if (_isPause || !_isInit)
            {
                return;
            }

            HandleInput();

            if (_isMoving)
            {
                Move();
            }
            else
            {
                if (_partyHandler.IsPartyMoving)
                {
                    _partyHandler.MoveParty();
                }
                else
                {
                    _isMoving = true;
                    _moveElapsedTime = 0f;
                    _origPos = this.transform.position;
                    _targetPos = _origPos + (_direction * 2);
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                UIManager.Instance.UIPause.Open();
            }

            if (Time.time - _lastDirectionChangeTime < 0.1f)
            {
                return;
            }
            if (Input.GetKey(KeyCode.W) && _direction != Vector3.down && _grid.HasTileInDirection(this.transform, Vector3.up))
            {
                _direction = Vector3.up;
                _lastDirectionChangeTime = Time.time;
            }
            if (Input.GetKey(KeyCode.S) && _direction != Vector3.up && _grid.HasTileInDirection(this.transform, Vector3.down))
            {
                _direction = Vector3.down;
                _lastDirectionChangeTime = Time.time;
            }
            if (Input.GetKey(KeyCode.A) && _direction != Vector3.right && _grid.HasTileInDirection(this.transform, Vector3.left))
            {
                _direction = Vector3.left;
                _lastDirectionChangeTime = Time.time;
            }
            if (Input.GetKey(KeyCode.D) && _direction != Vector3.left && _grid.HasTileInDirection(this.transform, Vector3.right))
            {
                _direction = Vector3.right;
                _lastDirectionChangeTime = Time.time;
            }
        }

        private void Move()
        {
            _moveElapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(_moveElapsedTime / _timeToMove);
            this.transform.position = Vector3.Lerp(_origPos, _targetPos, time);
            if (time >= 1f)
            {
                _isMoving = false;
                _partyHandler.StartPartyMove();
            }
        }

        public void ChangeDirection()
        {
            if (!_isInit)
            {
                return;
            }

            Vector3 newDirection = default;

            if (_direction == Vector3.up || _direction == Vector3.down)
            {
                newDirection = (Random.value < 0.5f) ? Vector3.left : Vector3.right;
            }
            else if (_direction == Vector3.left || _direction == Vector3.right)
            {
                newDirection = (Random.value < 0.5f) ? Vector3.up : Vector3.down;
            }

            if (_grid.HasTileInDirection(this.transform, newDirection))
            {
                _direction = newDirection;
                _lastDirectionChangeTime = Time.time;
            }
            else
            {
                ChangeDirection();
            }
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

