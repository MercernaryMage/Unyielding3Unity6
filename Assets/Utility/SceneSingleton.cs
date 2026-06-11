using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
            }
            return _instance;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _instance = null;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}