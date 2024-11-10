using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    [HideInInspector] public static T Instance = default;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Type t = typeof(T);
            Instance = (T)FindFirstObjectByType(t);
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// シーンがロードされたときに呼ばれる
    /// </summary>
    /// <param name="s"></param>
    /// <param name="mode"></param>
    public virtual void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {

    }
}
