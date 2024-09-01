using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SnakeGame.Manager;

namespace SnakeGame.UI
{
    public class UIPause : MonoBehaviour
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _mainMenuButton;

        public void Open()
        {
            PauseManager.Instance.Pause();
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            PauseManager.Instance.Resume();
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _resumeButton.onClick.AddListener(OnResumeButtonClicked);
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnResumeButtonClicked()
        {
            Close();
        }

        private void OnMainMenuButtonClicked()
        {
            SceneManager.LoadScene(SceneName.SCENE_START);
            PauseManager.Instance.ClearData();
            this.gameObject.SetActive(false);
        }

    }
}