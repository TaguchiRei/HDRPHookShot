using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField] PlayerMove _player;
    [SerializeField] float _moveSpeed = 1;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip[] _clip;

    private bool railGunShotted = false;
    private PlayerInput _playerInput;
    Mode _mode = Mode.submachineGun;
    public float _catch = 0;

    bool _destroyPlayerInput = false;
    void Start()
    {
        _destroyPlayerInput = false;
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

        _playerInput.Player.menu.started += InMenu;

        // アクションを有効化
        _playerInput.Enable();
    }

    private void Update()
    {
        if (_catch >= 0)
        {
            _catch -= Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        _destroyPlayerInput = true;
        _playerInput?.Dispose();
    }
    // ここから先は入力受付のためのプログラム

    /// <summary>
    /// 移動のための処理を書く。movePowerの値をここで変更。
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            Vector2 vector2 = context.ReadValue<Vector2>() * _moveSpeed;
            _player.MovePower = new Vector3(vector2.x, 0, vector2.y);
            if (context.phase == InputActionPhase.Started)
            {
                _player.Moving = true;
                if (_player.OnGround)
                {
                    _player.AnimationChange("Run", true);
                }
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                _player.Moving = false;
                _player.AnimationChange("Run", false);
            }
        }
    }
    /// <summary>
    /// 視点操作のためのプログラムを書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnLook(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            //ゲームパッドとマウスで操作を分ける。キーボードマウスでは視点操作が早すぎるため、20で割っている。
            if (context.control.device is Gamepad)
            {
                _player.Look = context.ReadValue<Vector2>() * 5;
            }
            else
            {
                _player.Look = context.ReadValue<Vector2>() / 20f;
            }
            //実際に回す動きはUpdate内で行う。
        }
    }
    /// <summary>
    /// ジャンプのための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            if (!_player._gameManager._pause)
            {
                if (context.phase == InputActionPhase.Started)
                {
                    _player.Jumping = true;
                }
                else if (context.phase == InputActionPhase.Canceled)
                {
                    _player.Jumping = false;
                }
            }
        }
    }
    /// <summary>
    /// 射撃のための操作をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnShot(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            if (!_player._gameManager._pause)
            {
                if (context.phase == InputActionPhase.Started)
                {
                    if (_mode == Mode.submachineGun)
                    {
                        if (_player.CanAction)
                        {
                            _player.Shotting = true;
                        }
                    }
                    else
                    {
                        if (_player.CanAction)
                        {
                            _player.CanAction = false;
                            railGunShotted = true;
                            _player.AnimationChange("R_Shot");
                            _audioSource.pitch = 1;
                            _audioSource.PlayOneShot(_clip[1]);
                            StartCoroutine(_player.ShotRailGun());
                        }
                    }
                }
                else if (context.phase == InputActionPhase.Canceled)
                {
                    _player.Shotting = false;
                }
            }
        }
    }

    /// <summary>
    /// エイムボタンを押した時の操作をここに書く。エイムの処理とフックショットを飛ばす処理。
    /// </summary>
    /// <param name="context"></param>
    private void OnAimAndHookShot(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            if (!_player._gameManager._pause)
            {
                if (context.phase == InputActionPhase.Started)
                {
                    if (_mode == Mode.submachineGun)
                    {
                        if (_player.CanAction)
                        {
                            _player.AnimationChange("HookShotOrAim");
                            _player.CanAction = false;
                            _player.AncShot();
                        }
                    }
                    else if (!railGunShotted)
                    {
                        _player.SensitivityCorrection = 0.3f;
                        _player.AnimationChange("HookShotOrAim");
                        _player.CanAction = true;
                    }
                }
                else if (context.phase == InputActionPhase.Canceled)
                {
                    _player.AnimationChange("HookShotOrAim", false);
                    _player.SensitivityCorrection = 1;
                    if (_mode == Mode.submachineGun)
                    {
                        _player.HookShotHit = false;
                        _player.AncDestroy();
                        _player.CanAction = true;
                    }
                    else
                    {
                        _player.CanAction = false;
                    }

                }
            }
        }
    }
    /// <summary>
    /// 武器の変形のための処理をここに書く。
    /// </summary>
    /// <param name="context"></param>
    private void OnConvert(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            if (_player.CanAction && !_player._gameManager._pause)
            {
                _player.AnimationChange("RailGunMode");
                _mode = Mode.railgun;
                railGunShotted = false;
                _player.CanAction = false;
                _player.Shotting = false;
            }
        }
    }
    /// <summary>
    /// アビリティ１を呼び出す処理
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility1(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            if (_player.CanAction && !_player._gameManager._pause)
            {
                _player.CanAction = false;
                Debug.Log(_player.AbilitySetting.abilityNumber1);
                _player.UseAbility(_player.AbilitySetting.abilityNumber1);
            }
        }
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

    private void InMenu(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            if (!_player._gameManager._pause)
            {
                _player._gameManager.Stop();
                _player._gameManager.OpenMenu();
            }
            else
            {
                _player._gameManager.ReStart();
            }
        }
    }



    public enum Mode
    {
        submachineGun,
        railgun,
    }
    public void ModeReset()
    {
        _mode = Mode.submachineGun;
        _player.CanAction = true;
    }

    public void DMGSound()
    {
        _audioSource.volume = 0.2f;
        _audioSource.pitch = 2f;
        if (!_audioSource.isPlaying)
            _audioSource.PlayOneShot(_clip[2]);
        _audioSource.volume = 0.4f;
    }
}
