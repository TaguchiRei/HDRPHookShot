using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    GameManager _gameManager;
    public bool _canMove = false;
    [SerializeField] float _moveSpeed = 1;
    public bool _canJump = false;
    [SerializeField] float _jumpPower = 1;

    [SerializeField] float _gravityScale = 1;
    [SerializeField] float _gravityScaleChangePoint;
    [SerializeField] GameObject _playerBody;
    [SerializeField] Rigidbody _rigidbody;
    Vector3 _movePower = Vector3.zero;

    private float _groundDistanse = 0;
    private PlayerInput _playerInput;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        _playerInput = new();

        //アクションイベントを登録

        // Moveアクションのイベント登録
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.started += OnMove;
        _playerInput.Player.Move.canceled += OnMove;

        // Lookアクションのイベント登録
        _playerInput.Player.Look.performed += OnLook;
        _playerInput.Player.Look.started += OnLook;
        _playerInput.Player.Look.canceled += OnLook;

        // Jumpアクションのイベント登録
        _playerInput.Player.Jump.performed += OnJump;
        _playerInput.Player.Jump.started += OnJump;
        _playerInput.Player.Jump.canceled += OnJump;

        // Shotアクションのイベント登録
        _playerInput.Player.Shot.performed += OnShot;
        _playerInput.Player.Shot.started += OnShot;
        _playerInput.Player.Shot.canceled += OnShot;

        // Interactアクションのイベント登録
        _playerInput.Player.Interact.performed += OnInteract;
        _playerInput.Player.Interact.started += OnInteract;
        _playerInput.Player.Interact.canceled += OnInteract;

        // Aimアクションのイベント登録
        _playerInput.Player.Aim.performed += OnAim;
        _playerInput.Player.Aim.started += OnAim;
        _playerInput.Player.Aim.canceled += OnAim;

        // Convertアクションのイベント登録
        _playerInput.Player.Convert.performed += OnConvert;
        _playerInput.Player.Convert.started += OnConvert;
        _playerInput.Player.Convert.canceled += OnConvert;

        // Ability1アクションのイベント登録
        _playerInput.Player.Ability1.performed += OnAbility1;
        _playerInput.Player.Ability1.started += OnAbility1;
        _playerInput.Player.Ability1.canceled += OnAbility1;

        // Ability2アクションのイベント登録
        _playerInput.Player.Ability2.performed += OnAbility2;
        _playerInput.Player.Ability2.started += OnAbility2;
        _playerInput.Player.Ability2.canceled += OnAbility2;

        // Ability3アクションのイベント登録
        _playerInput.Player.Ability3.performed += OnAbility3;
        _playerInput.Player.Ability3.started += OnAbility3;
        _playerInput.Player.Ability3.canceled += OnAbility3;

        // アクションを有効化
        _playerInput.Enable();
    }

    void Update()
    {
        //プレイヤーの動きを作る

    }
    private void FixedUpdate()
    {
        //プレイヤーの動きを作る
        _rigidbody.AddForce(_movePower);
        //重力を作る
        Physics.BoxCast(transform.position, transform.localScale / 2, Vector3.down, out RaycastHit hit, Quaternion.identity);
        _groundDistanse = hit.distance;
        //重力加速度に重力の強さをかけ、落ちる時はさらに重力を増加させる。地面に近づくとさらに強力に
        Vector3 gravity = new Vector3(0, -9.81f, 0) * _gravityScale;
        gravity = _rigidbody.linearVelocity.y < 0 ? gravity * 1.2f : gravity;
        gravity = _groundDistanse < _gravityScaleChangePoint ? gravity * 2f : gravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnDestroy()
    {
        _playerInput?.Dispose();
    }


    /* ここから先は入力受付のためのプログラム*/

    /// <summary>
    /// 移動のための処理を書く。movePowerの値をここで変更。
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>() * _moveSpeed;
        _movePower = new Vector3(vector2.x, 0, vector2.y);
    }
    /// <summary>
    /// 視点操作のためのプログラムを書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnLook(InputAction.CallbackContext context)
    {
        //回転速度を調節するために20で割っている。また、デフォルト値が反転操作なのでX軸回転は-1をかける
        Vector2 look = context.ReadValue<Vector2>() / 20f;
        transform.Rotate(0,look.x * _gameManager._horiaontalCamera,0);
        _playerBody.transform.Rotate(look.y * _gameManager._verticalCamera * -1f, 0, 0);
    }
    /// <summary>
    /// ジャンプのための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// 射撃のための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnShot(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// インタラクトの操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnInteract(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// エイムボタンを押した時の操作をここに書く。エイムの処理とフックショットを飛ばす処理。
    /// </summary>
    /// <param name="context"></param>
    private void OnAim(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// 武器の変形のための処理をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnConvert(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// アビリティ１を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility1(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// アビリティ２を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility2(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// アビリティ３を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility3(InputAction.CallbackContext context)
    {

    }
}
