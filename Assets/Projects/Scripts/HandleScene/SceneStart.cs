using SnakeGame.Manager;
using UnityEngine;

namespace SnakeGame
{
    public class SceneStart : MonoBehaviour
    {
        private void Start()
        {
            UIManager.Instance.UIMainMenu.Open();
            UIManager.Instance.UIPause.Close();
            UIManager.Instance.UIResult.Close();
        }
    }
}
