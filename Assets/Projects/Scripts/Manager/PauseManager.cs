using System.Collections.Generic;
using SnakeGame.Interface;
using UnityEngine;

namespace SnakeGame.Manager
{
    public sealed class PauseManager : Singleton<PauseManager>
    {
        [SerializeField] private bool _isPaused = false;
        private List<IPausable> _pausables = new List<IPausable>();

        public void RegisterPausable(IPausable pausable)
        {
            if (!_pausables.Contains(pausable))
            {
                _pausables.Add(pausable);
            }
        }

        public void UnregisterPausable(IPausable pausable)
        {
            if (_pausables.Contains(pausable))
            {
                _pausables.Remove(pausable);
            }
        }

        public void Pause()
        {
            _isPaused = true;

            foreach (var pausable in _pausables)
            {
                pausable.OnPause();
            }
        }

        public void Resume()
        {
            _isPaused = false;

            foreach (var pausable in _pausables)
            {
                pausable.OnResume();
            }
        }

        public bool IsPaused()
        {
            return _isPaused;
        }

        public void ClearData()
        {
            _pausables.Clear();
        }
    }
}

