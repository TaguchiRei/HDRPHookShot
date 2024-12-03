using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField] PlayerMove _player;
    [SerializeField] float _moveSpeed = 1;

    private PlayerInput _playerInput;
    Mode _mode = Mode.submachineGun;
    void Start()
    {
        _playerInput = new();

        //アクションイベントを登録
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.started += OnMove;
        _playerInput.Player.Move.canceled += OnMove;

        _playerInput.Player.Look.performed += OnLook;
        _playerInput.Player.Look.started += OnLook;
        _playerInput.Player.Look.canceled += OnLook;

        _playerInput.Player.Jump.canceled += OnJump;
        _playerInput.Player.Jump.started += OnJump;

        _playerInput.Player.Shot.performed += OnShot;
        _playerInput.Player.Shot.started += OnShot;
        _playerInput.Player.Shot.canceled += OnShot;

        _playerInput.Player.Interact.performed += OnInteract;
        _playerInput.Player.Interact.started += OnInteract;
        _playerInput.Player.Interact.canceled += OnInteract;

        _playerInput.Player.Aim.started += OnAimAndHookShot;
        _playerInput.Player.Aim.canceled += OnAimAndHookShot;

        _playerInput.Player.Convert.performed += OnConvert;
        _playerInput.Player.Convert.started += OnConvert;
        _playerInput.Player.Convert.canceled += OnConvert;

        _playerInput.Player.Ability1.performed += OnAbility1;
        _playerInput.Player.Ability1.started += OnAbility1;
        _playerInput.Player.Ability1.canceled += OnAbility1;

        _playerInput.Player.Ability2.performed += OnAbility2;
        _playerInput.Player.Ability2.started += OnAbility2;
        _playerInput.Player.Ability2.canceled += OnAbility2;

        _playerInput.Player.Ability3.performed += OnAbility3;
        _playerInput.Player.Ability3.started += OnAbility3;
        _playerInput.Player.Ability3.canceled += OnAbility3;

        // アクションを有効化
        _playerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        _playerInput?.Dispose();
    }
    // ここから先は入力受付のためのプログラム

    /// <summary>
    /// 移動のための処理を書く。movePowerの値をここで変更。
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>() * _moveSpeed;
        _player._movePower = new Vector3(vector2.x, 0, vector2.y);
        if (context.phase == InputActionPhase.Started)
        {
            _player._moving = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _player._moving = false;
        }
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
            _player.look = context.ReadValue<Vector2>() * 5;
        }
        else
        {
            _player.look = context.ReadValue<Vector2>() / 20f;
        }
        //実際に回す動きはUpdate内で行う。
    }
    /// <summary>
    /// ジャンプのための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _player._jumping = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _player._jumping =false;
        }
    }
    /// <summary>
    /// 射撃のための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnShot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (_mode == Mode.submachineGun)
            {
                if (_player._canAction)
                {
                    _player._shotting = true;
                }
            }
            else
            {
                if (_player._canAction)
                {
                    _player.AnimationChange("R_Shot");
                }
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _player._shotting = false;
        }
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
    private void OnAimAndHookShot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _player.AnimationChange("HookShotOrAim");
            if (_mode == Mode.submachineGun)
            {
                _player._canAction = false;
                _player.AncShot();
            }
            else
            {
                _player._canAction = true;
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _player.AnimationChange("HookShotOrAim", false);
            if (_mode == Mode.submachineGun)
            {
                _player._hookShotHit = false;
                _player.AncDestroy();
                _player._canAction = true;
            }
            else
            {
                _player._canAction = false;
            }
        }
    }
    /// <summary>
    /// 武器の変形のための処理をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnConvert(InputAction.CallbackContext context)
    {
        if (_player._canAction)
        {
            _player.AnimationChange("RailGunMode");
            _mode = Mode.railgun;
            _player._canAction = false;
            _player._shotting = false;
        }
    }
    /// <summary>
    /// アビリティ１を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility1(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility1");
        if (_player._canAction)
        {
            _player._canAction = false;
            Ability(0);
        }
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

    public enum Mode
    {
        submachineGun,
        railgun,

    }
    public void ModeReset()
    {
        _mode = Mode.submachineGun;
        _player._canAction = true;
    }
    void Ability(int abilityNumber)
    {
        _player.UseAbility(abilityNumber);
    }
}
