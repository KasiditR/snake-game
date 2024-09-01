using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SnakeGame.Manager;

namespace SnakeGame.UI
{
    public class UIResult : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hightScoreText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _replayButton;
        [SerializeField] private Button _mainMenuButton;

        private int _score;

        public void Open(int score)
        {
            _score = score;
            SetScoreText(_score.ToString());
            SetHightScoreText();
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            _hightScoreText.text = string.Empty;
            _scoreText.text = string.Empty;
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _replayButton.onClick.AddListener(OnReplayButtonClicked);
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnDisable()
        {
            _replayButton.onClick.RemoveAllListeners();
            _mainMenuButton.onClick.RemoveAllListeners();
        }

        private void OnReplayButtonClicked()
        {
            SceneManager.LoadScene(SceneName.SCENE_GAMEPLAY);
            UIManager.Instance.UIGameplay.SetScoreText(string.Empty);
            Close();
        }

        private void OnMainMenuButtonClicked()
        {
            SceneManager.LoadScene(SceneName.SCENE_START);
            Close();
        }

        public void SetScoreText(string scoreText)
        {
            _scoreText.text = $"Score : {scoreText}";
        }

        public void SetHightScoreText()
        {
            if (!PlayerPrefs.HasKey(PlayerPefKey.HIGHT_SCORE))
            {
                _hightScoreText.text = $"High Score : {_score}";
                SaveHightScore();
            }
            else
            {
                int hightScore = PlayerPrefs.GetInt(PlayerPefKey.HIGHT_SCORE);
                if (hightScore < _score)
                {
                    _hightScoreText.text = $"High Score : {_score}";
                    SaveHightScore();
                }
                else
                {
                    _hightScoreText.text = $"High Score : {hightScore}";
                }
            }
        }

        private void SaveHightScore()
        {
            PlayerPrefs.SetInt(PlayerPefKey.HIGHT_SCORE, _score);
        }
    }
}