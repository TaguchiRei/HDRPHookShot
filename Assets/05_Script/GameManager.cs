using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance = default;
    /// <summary>
    /// �J�����̐��������̔��]�̉ۂ��s���B�P�ɐ��l��������΃J�����̊��x���ς��
    /// </summary>
    [Range (-1,1)] public int _horiaontalCamera = 0;
    /// <summary>
    /// �J�����̐��������̔��]�̉ۂ��s���B�P�ɐ��l��������΃J�����̊��x���ς��
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
        //�}�E�X���Œ肵�ĉB��
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �V�[�������[�h���ꂽ�Ƃ��ɌĂ΂��
    /// </summary>
    /// <param name="s"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {

    }
}
