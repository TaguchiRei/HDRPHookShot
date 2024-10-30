using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance = default;
    /// <summary>
    /// カメラの水平方向の反転の可否を行う。単に数値を下げればカメラの感度が変わる
    /// </summary>
    [Range (-1,1)] public int _horiaontalCamera = 0;
    /// <summary>
    /// カメラの垂直方向の反転の可否を行う。単に数値を下げればカメラの感度が変わる
    /// </summary>
    [Range(-1, 1)] public int _verticalCamera = 0;

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
        //マウスを固定して隠す
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
