using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SnakeGame.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hightScoreText;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;

        private const string SCENE_GAMEPLAY = "Gameplay";

        public void Open()
        {
            SetHightScoreText();
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);

        }

        private void OnDisable()
        {
            _startButton.onClick.RemoveAllListeners();
            _quitButton.onClick.RemoveAllListeners();
        }

        private void OnStartButtonClicked()
        {
            SceneManager.LoadScene(SCENE_GAMEPLAY);
            Close();
        }

        private void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        private void SetHightScoreText()
        {
            if (!PlayerPrefs.HasKey(PlayerPefKey.HIGHT_SCORE))
            {
                _hightScoreText.gameObject.SetActive(false);
            }
            else
            {
                int hightScore = PlayerPrefs.GetInt(PlayerPefKey.HIGHT_SCORE);
                if (hightScore <= 0)
                {
                    _hightScoreText.gameObject.SetActive(false);
                }
                else
                {
                    _hightScoreText.text = $"High Score : {hightScore}";
                    _hightScoreText.gameObject.SetActive(true);
                }
            }
        }
    }
}
