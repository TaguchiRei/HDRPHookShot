using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance = default;
    

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// シーンがロードされたときに呼ばれる
    /// </summary>
    /// <param name="s"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {

    }
}
