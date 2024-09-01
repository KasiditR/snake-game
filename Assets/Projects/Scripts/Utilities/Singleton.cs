using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool _isDestroyed = false;
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_isDestroyed)
            {
                return null;
            }

            return _instance;
        }

        private set { _instance = value; }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = GetComponent<T>();
            DontDestroyOnLoad(this);
            _isDestroyed = false;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _isDestroyed = true;
        }
    }
}
