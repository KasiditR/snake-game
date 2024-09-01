using SnakeGame.Character;
using UnityEngine;

namespace SnakeGame.Controller
{
    public class PlayerCollisionHandler : MonoBehaviour
    {
        private PartyHandler _partyHandle;
        private PlayerController _playerController;

        public void Initialize(PlayerController playerController, PartyHandler partyHandler)
        {
            _partyHandle = partyHandler;
            _playerController = playerController;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.TryGetComponent<AdventurerCharacter>(out AdventurerCharacter adventurer))
            {
                _partyHandle?.AddMember(adventurer);
            }

            if (other.gameObject.CompareTag(TagName.WALL))
            {
                HandleWallCollision();
            }
        }

        private void HandleWallCollision()
        {
            _partyHandle.PartyMembers[0].DieImmediate();
            _playerController.ChangeDirection();
        }
    }
}
