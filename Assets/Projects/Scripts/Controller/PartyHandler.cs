using System.Collections.Generic;
using UnityEngine;
using SnakeGame.Character;
using SnakeGame.Manager;
using System.Linq;

namespace SnakeGame.Controller
{
    public class PartyHandler : MonoBehaviour
    {
        [SerializeField] private List<AdventurerCharacter> _partyMembers;
        private List<Vector3> _collectPos;
        private PlayerController _playerController;
        private float _partyMoveElapsedTime;
        private int _currentPartyIndex;
        private bool _isPartyMoving;

        public List<AdventurerCharacter> PartyMembers { get => _partyMembers; }
        public bool IsPartyMoving { get => _isPartyMoving; }

        public void Initialize(PlayerController playerController)
        {
            _playerController = playerController;
            AdjustMovementSpeed();
        }

        public void StartPartyMove()
        {
            if (_partyMembers.Count > 0)
            {
                _isPartyMoving = true;
                _partyMoveElapsedTime = 0f;
                _currentPartyIndex = 0;
                _collectPos = _partyMembers.Select(x => x.transform.position).ToList();
            }
        }

        public void MoveParty()
        {
            _partyMoveElapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(_partyMoveElapsedTime / 0.05f);
            Vector3 startPos = _partyMembers[_currentPartyIndex].transform.position;
            Vector3 targetPos = _currentPartyIndex == 0 ? this.transform.position : _collectPos[_currentPartyIndex - 1];
            Vector3 dir = targetPos - startPos;
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

        public void AddMember(AdventurerCharacter adventurer)
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

        public void RemoveMember(AdventurerCharacter adventurer)
        {
            if (_partyMembers.Contains(adventurer))
            {
                _partyMembers.Remove(adventurer);
                AdjustMovementSpeed();
            }
        }

        private void OnAdventurerDie(BaseCharacter character)
        {
            RemoveMember(character as AdventurerCharacter);

            if (_partyMembers.Count <= 0)
            {
                _playerController.IsPause = true;
                CoreGameManager.Instance.GameManager.GameOver();
            }

            character.OnDie -= OnAdventurerDie;
        }

        private void AdjustMovementSpeed()
        {
            if (_playerController == null)
            {
                return;
            }

            int partySize = Mathf.Max(1, _partyMembers.Count);
            Vector2 minMaxSpeed = CoreGameManager.Instance.GameConfig.MinMaxSpeed;
            _playerController.TimeToMove = Mathf.Clamp(minMaxSpeed.y / partySize, minMaxSpeed.x, minMaxSpeed.y);
        }
    }
}
