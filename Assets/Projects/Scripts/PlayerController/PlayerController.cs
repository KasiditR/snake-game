using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using SnakeGame.Character;
using SnakeGame.Manager;
using SnakeGame.Interface;

namespace SnakeGame.Controller
{
    public class PlayerController : MonoBehaviour, IPausable
    {
        [SerializeField] private float _timeToMove;
        [SerializeField] private List<AdventurerCharacter> _partyMembers;
        private float _lastDirectionChangeTime;
        private float _moveElapsedTime;
        private float _partyMoveElapsedTime;
        private int _currentPartyIndex;
        private bool _isMoving;
        private bool _isPartyMoving;
        private bool _isInit;
        private bool _isPause;
        private GridTile _grid;
        private GameManager _gameManager;
        private Vector3 _origPos;
        private Vector3 _targetPos;
        private Vector3 _direction;
        private List<Vector3> _collectPos = new List<Vector3>();

        public void Initialize(GameManager gameManager)
        {
            this._gameManager = gameManager;
            _grid = _gameManager.Grid;
            _direction = Vector3.down;
            _isInit = true;
            AdjustMovementSpeed();
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
            if (_isPause)
            {
                return;
            }

            if (!_isInit)
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
                if (_isPartyMoving)
                {
                    MoveParty();
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
                StartPartyMove();
            }
        }

        private void StartPartyMove()
        {
            if (_partyMembers.Count > 0)
            {
                _isPartyMoving = true;
                _partyMoveElapsedTime = 0f;
                _currentPartyIndex = 0;
                _collectPos = _partyMembers.Select(x => x.transform.position).ToList();
            }
        }

        private void MoveParty()
        {
            _partyMoveElapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(_partyMoveElapsedTime / 0.05f);
            Vector3 startPos = _partyMembers[_currentPartyIndex].transform.position;
            Vector3 targetPos = _currentPartyIndex == 0 ? this.transform.position : _collectPos[_currentPartyIndex - 1];
            Vector3 dir = (targetPos - startPos);
            _partyMembers[_currentPartyIndex].SetDirection(dir.normalized);
            _partyMembers[_currentPartyIndex].transform.position = Vector3.Lerp(startPos, targetPos, time);

            if (_currentPartyIndex == 0)
            {
                _partyMembers[0].SetLeader();
            }
            else
            {
                _partyMembers[_currentPartyIndex].SetMember();
            }

            if (time >= 1f)
            {
                _partyMoveElapsedTime = 0f;
                _currentPartyIndex++;

                if (_currentPartyIndex >= _partyMembers.Count)
                {
                    _isPartyMoving = false;
                }
            }
        }

        public void AddMemberParty(AdventurerCharacter adventurer)
        {
            if (adventurer.IsDie)
            {
                return;
            }

            if (!_partyMembers.Contains(adventurer))
            {
                _partyMembers.Add(adventurer);
                adventurer.OnDie += OnAdventurerDie;
                AdjustMovementSpeed();
            }
        }

        private void OnAdventurerDie(BaseCharacter character)
        {
            if (_partyMembers.Contains(character))
            {
                _partyMembers.Remove(character as AdventurerCharacter);
                AdjustMovementSpeed();
            }

            character.OnDie -= OnAdventurerDie;

            if (_partyMembers.Count <= 0)
            {
                _isPause = true;
                this._gameManager.GameOver();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent<AdventurerCharacter>(out AdventurerCharacter adventurer))
            {
                AddMemberParty(adventurer);
            }

            if (other.gameObject.CompareTag(TagName.WALL))
            {
                HitWall();
            }
        }

        private void HitWall()
        {
            _partyMembers[0].DieImmediate();
            ChangeDirection();
        }

        private void ChangeDirection()
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

        private void AdjustMovementSpeed()
        {
            int partySize = Mathf.Max(1, _partyMembers.Count);
            Vector2 minMaxSpeed = CoreGameManager.Instance.GameConfig.MinMaxSpeed;
            _timeToMove = Mathf.Clamp(minMaxSpeed.y / partySize, minMaxSpeed.x, minMaxSpeed.y);
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

