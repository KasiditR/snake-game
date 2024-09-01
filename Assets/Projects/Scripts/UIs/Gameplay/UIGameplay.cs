using TMPro;
using UnityEngine;

namespace SnakeGame.UI
{
    public class UIGameplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _countdownText;

        public void SetScoreText(string scoreText)
        {
            _scoreText.text = $"Score : {scoreText}";
        }

        public void SetCountdownText(string countdownText)
        {
            _countdownText.text = countdownText;
            _countdownText.gameObject.SetActive(countdownText != string.Empty);
        }
    }
}