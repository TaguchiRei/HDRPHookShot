using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    GameManager _gameManager;
    public bool _canMoveInput = false;
    [SerializeField] float _moveSpeed = 1;
    public bool _canJumpInput = false;
    [SerializeField] float _jumpPower = 1;
    public bool _canUseWeapon = false;
    [SerializeField] WeaponStatus _weaponStatus;
    
    [SerializeField] float _gravityScale = 1;
    [SerializeField] float _gravityScaleChangePoint;
    [SerializeField] GameObject _playerBody;
    [SerializeField] Rigidbody _rigidbody;
    Vector3 _movePower = Vector3.zero;
    Vector2 look;

    bool _canJump = false;
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
        //視点操作はここで行う。
        transform.Rotate(0, look.x * _gameManager._horiaontalCamera, 0);
        //上下カメラの上限と下限を設定する。また、デフォルト値が反転操作なのでX軸回転(カメラの上下方向操作)は-1をかける
        float verticalAngle = Mathf.Clamp(look.y * _gameManager._verticalCamera * -1f, -80f, 80f);
        float playerAngle = _playerBody.transform.rotation.eulerAngles.x;
        //プレイヤーの角度をeulerAnglesで取得しているのでマイナスは360からその値を引いた数になるので360で引くことで正しい値に戻す。
        if (playerAngle > 180) playerAngle -= 360;
        if (verticalAngle + playerAngle < 80 && -80 < verticalAngle + playerAngle)
        {
            _playerBody.transform.Rotate(verticalAngle, 0, 0);
        }
    }
    private void FixedUpdate()
    {
        //プレイヤーの動きを作る
        _rigidbody.AddForce(transform.TransformDirection(_movePower));
        //重力を作る
        Physics.BoxCast(transform.position, new Vector3(transform.localScale.x,0.1f,transform.localScale.z), Vector3.down, out RaycastHit hit, Quaternion.identity);
        _groundDistanse = hit.distance;
        //重力加速度に重力の強さをかけ、落ちる時はさらに重力を増加させる。地面に近づくとさらに強力に
        Vector3 gravity = new Vector3(0, -9.81f, 0) * _gravityScale;
        gravity = _rigidbody.linearVelocity.y < 0 ? gravity * 1.2f : gravity;
        gravity = _groundDistanse < _gravityScaleChangePoint ? gravity * 2f : gravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        //接地判定のために地面との距離とベロシティを測る
        if (_groundDistanse < 0.1f && _rigidbody.linearVelocity.y <= 0)
        {
            _canJump = true;
        }
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
        //ゲームパッドとマウスで操作を分ける。キーボードマウスでは視点操作が早すぎるため、20で割っている。
        if (context.control.device is Gamepad)
        {
            look = context.ReadValue<Vector2>();
        }
        else
        {
            look =  context.ReadValue<Vector2>() / 20f;
        }
        //実際に回す動きはUpdate内で行う。
    }
    /// <summary>
    /// ジャンプのための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {
        //下に向けたボックスキャストで取得した地面との距離で接地判定をとる。このオブジェクトは足元が原点になっており、原点からボックスキャストを出しているので大きさを変えても常に地面との距離は一定。
        if (_groundDistanse < 0.1f && _canJump)
        {
            _rigidbody.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
            _canJump = false;
        }
    }
    /// <summary>
    /// 射撃のための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnShot(InputAction.CallbackContext context)
    {
        Debug.Log("ShotButton");
    }
    /// <summary>
    /// インタラクトの操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("InteractButton");
    }
    /// <summary>
    /// エイムボタンを押した時の操作をここに書く。エイムの処理とフックショットを飛ばす処理。
    /// </summary>
    /// <param name="context"></param>
    private void OnAim(InputAction.CallbackContext context)
    {
        Debug.Log("AimAndHookShotButton");
    }
    /// <summary>
    /// 武器の変形のための処理をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnConvert(InputAction.CallbackContext context)
    {
        Debug.Log("ConvertButton");
    }
    /// <summary>
    /// アビリティ１を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility1(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility1");
    }
    /// <summary>
    /// アビリティ２を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility2(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility2");
    }
    /// <summary>
    /// アビリティ３を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility3(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility3");
    }
}
