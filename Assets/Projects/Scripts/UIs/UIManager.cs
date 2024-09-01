using SnakeGame.UI;
using UnityEngine;

namespace SnakeGame.Manager
{
    public sealed class UIManager : Singleton<UIManager>
    {
        [SerializeField] private UIMainMenu uiMainMenu;
        [SerializeField] private UIGameplay uiGameplay;
        [SerializeField] private UIResult uiResult;
        [SerializeField] private UIPause uiPause;

        public UIMainMenu UIMainMenu { get => uiMainMenu; }
        public UIGameplay UIGameplay { get => uiGameplay; }
        public UIResult UIResult { get => uiResult; }
        public UIPause UIPause { get => uiPause; }
    }
}

