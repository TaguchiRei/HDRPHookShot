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

        //�A�N�V�����C�x���g��o�^
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

        // �A�N�V������L����
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
    // ���������͓��͎�t�̂��߂̃v���O����

    /// <summary>
    /// �ړ��̂��߂̏����������BmovePower�̒l�������ŕύX�B
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
    /// ���_����̂��߂̃v���O�����������B
    /// </summary>
    /// <param name="context"></param>
    private void OnLook(InputAction.CallbackContext context)
    {
        if (!_destroyPlayerInput)
        {
            //�Q�[���p�b�h�ƃ}�E�X�ő���𕪂���B�L�[�{�[�h�}�E�X�ł͎��_���삪�������邽�߁A20�Ŋ����Ă���B
            if (context.control.device is Gamepad)
            {
                _player.Look = context.ReadValue<Vector2>() * 5;
            }
            else
            {
                _player.Look = context.ReadValue<Vector2>() / 20f;
            }
            //���ۂɉ񂷓�����Update���ōs���B
        }
    }
    /// <summary>
    /// �W�����v�̂��߂̑���������ɏ����B
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
    /// �ˌ��̂��߂̑���������ɏ����B
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
    /// �G�C���{�^�������������̑���������ɏ����B�G�C���̏����ƃt�b�N�V���b�g���΂������B
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
    /// ����̕ό`�̂��߂̏����������ɏ����B
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
    /// �A�r���e�B�P���Ăяo������
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
    /// �A�r���e�B�Q���Ăяo������
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility2(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// �A�r���e�B�R���Ăяo������
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
