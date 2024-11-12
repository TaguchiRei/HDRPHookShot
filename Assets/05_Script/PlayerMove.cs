using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    GameManager _gameManager;
    public bool _canMoveInput = false;
    [SerializeField] float _moveSpeed = 1;
    public bool _canJumpInput = false;
    [SerializeField] float _jumpPower = 1;
    public bool _canUseWeaponInput = false;
    public WeaponStatus _weaponStatus;
    [SerializeField] float _gravityScale = 1;
    [SerializeField] float _gravityScaleChangePoint;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject _Anchor;
    [SerializeField] GameObject _playerBody;
    [SerializeField] Animator _anim;
    [SerializeField] Rigidbody _rigidbody;

    //�ʏ�ˌ��̂��߂̕ϐ�
    [SerializeField] UnityEvent _shot;
    bool _shotting = false;
    [SerializeField] float _defaultShotInterval = 0.2f;
    float _shotInterval = 0.2f;
    float anchorTimer = 0;

    Vector3 _movePower = Vector3.zero;
    Vector2 look;

    bool _canJump = false;
    private float _groundDistance = 0;
    private PlayerInput _playerInput;

    //�v���C���[�̏�Ԃ�ۑ�����ϐ�
    Mode _mode = Mode.submachineGun;
    bool _canShotConvert = true;//�ʏ�ˌ��A�R���o�[�g�A���[���K���ˌ����Ǘ�
    bool _canUseAbility = false;
    bool _movingNow = false;
    bool _jumping = false;



    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        _playerInput = new();

        //�A�N�V�����C�x���g��o�^
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.started += OnMove;
        _playerInput.Player.Move.canceled += OnMove;

        _playerInput.Player.Look.performed += OnLook;
        _playerInput.Player.Look.started += OnLook;
        _playerInput.Player.Look.canceled += OnLook;

        _playerInput.Player.Jump.performed += OnJump;
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

        // �A�N�V������L����
        _playerInput.Enable();
    }

    void Update()
    {
        //�v���C���[�̓��������
        //���_����͂����ōs���B
        transform.Rotate(0, look.x * _gameManager._horizontalCamera, 0);
        //�㉺�J�����̏���Ɖ�����ݒ肷��B�܂��A�f�t�H���g�l�����]����Ȃ̂�X����](�J�����̏㉺��������)��-1��������
        float verticalAngle = Mathf.Clamp(look.y * _gameManager._verticalCamera * -1f, -80f, 80f);
        float playerAngle = _playerBody.transform.rotation.eulerAngles.x;
        //�v���C���[�̊p�x��EulerAngles�Ŏ擾���Ă���̂Ń}�C�i�X��360���炻�̒l�����������ɂȂ�̂�360�ň������ƂŐ������l�ɖ߂��B
        if (playerAngle > 180) playerAngle -= 360;
        if (verticalAngle + playerAngle < 80 && -80 < verticalAngle + playerAngle)
        {
            _playerBody.transform.Rotate(verticalAngle, 0, 0);
        }

        //�ˌ��v���O����
        if (_shotting && _shotInterval <=0)
        {
            _shot.Invoke();
            _shotInterval = _defaultShotInterval;
        }
        if (_shotInterval > 0)
        {
            _shotInterval -= Time.deltaTime;
        }
        if (anchorTimer > 0)
        {
            anchorTimer -= Time.deltaTime;
            if (anchorTimer < 0)
            {
                _Anchor.GetComponent<Anchor>().AnchorReset();
                _anim.SetBool("HookShotOrAim", false);
                _canShotConvert = true;
            }
        }
    }
    private void FixedUpdate()
    {
        //�v���C���[�̓��������
        if(_groundDistance < 0.1f && _movingNow && _movePower != Vector3.zero)
        {
            Vector3 move = transform.TransformDirection(_movePower);
            _rigidbody.linearVelocity = new Vector3(move.x,_rigidbody.linearVelocity.y,move.z);
        }

        //�d�͂����
        Physics.BoxCast(transform.position, new Vector3(transform.localScale.x,0.1f,transform.localScale.z), Vector3.down, out RaycastHit hit, Quaternion.identity);
        _groundDistance = hit.distance;
        //�d�͉����x�ɏd�͂̋����������A�����鎞�͂���ɏd�͂𑝉�������B�n�ʂɋ߂Â��Ƃ���ɋ��͂�
        Vector3 gravity = new Vector3(0, -9.81f, 0) * _gravityScale;
        gravity = _rigidbody.linearVelocity.y < 0 ? gravity * 1.2f : gravity;
        gravity = _groundDistance < _gravityScaleChangePoint ? gravity * 2f : gravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        //�ڒn����̂��߂ɒn�ʂƂ̋����ƃx���V�e�B�𑪂�
        if (_groundDistance < 0.1f && _rigidbody.linearVelocity.y <= 0)
        {
            _canJump = true;
        }
        //�W�����v�̏���
        if (_canJump && _jumping)
        {
            _rigidbody.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
            _canJump = false;
        }
    }

    private void OnDestroy()
    {
        _playerInput?.Dispose();
    }


    // ���������͓��͎�t�̂��߂̃v���O����

    /// <summary>
    /// �ړ��̂��߂̏����������BmovePower�̒l�������ŕύX�B
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>() * _moveSpeed;
        _movePower = new Vector3(vector2.x, 0, vector2.y);
        if (context.phase == InputActionPhase.Started)
        {
            _movingNow = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _movingNow = false;
        }
    }
    /// <summary>
    /// ���_����̂��߂̃v���O�����������B
    /// </summary>
    /// <param name="context"></param>
    private void OnLook(InputAction.CallbackContext context)
    {
        //�Q�[���p�b�h�ƃ}�E�X�ő���𕪂���B�L�[�{�[�h�}�E�X�ł͎��_���삪�������邽�߁A20�Ŋ����Ă���B
        if (context.control.device is Gamepad)
        {
            look = context.ReadValue<Vector2>() * 5;
        }
        else
        {
            look =  context.ReadValue<Vector2>() / 20f;
        }
        //���ۂɉ񂷓�����Update���ōs���B
    }
    /// <summary>
    /// �W�����v�̂��߂̑���������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _jumping = true;
        }
        else
        {
            _jumping = false;
        }
    }
    /// <summary>
    /// �ˌ��̂��߂̑���������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnShot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (_mode == Mode.submachineGun)
            {
                if (_canShotConvert)
                {
                    _shotting = true;
                }
            }
            else
            {
                if (_canShotConvert)
                {
                    _anim.SetBool("R_Shot", true);
                }
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _shotting = false;
        }
    }
    /// <summary>
    /// �C���^���N�g�̑���������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("InteractButton");
    }
    /// <summary>
    /// �G�C���{�^�������������̑���������ɏ����B�G�C���̏����ƃt�b�N�V���b�g���΂������B
    /// </summary>
    /// <param name="context"></param>
    private void OnAimAndHookShot(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            _anim.SetBool("HookShotOrAim", true);
            if(_mode == Mode.submachineGun)
            {
                anchorTimer = _weaponStatus.HookShotTimer;
                _Anchor.GetComponent<Anchor>().AnchorShot(_weaponStatus.HookShotSpeed);
                _canShotConvert = false;
            }
            else
            {
                _canShotConvert = true;
            }
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            _anim.SetBool("HookShotOrAim", false);
            if (_mode == Mode.submachineGun)
            {
                _Anchor.GetComponent<Anchor>().AnchorReset(gameObject);
                _canShotConvert = true;
            }
            else
            {
                _canShotConvert = false;
            }
        }
    }
    /// <summary>
    /// ����̕ό`�̂��߂̏����������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnConvert(InputAction.CallbackContext context)
    {
        if (_canShotConvert)
        {
            _anim.SetBool("RailGunMode", true);
            _mode = Mode.railgun;
            _canShotConvert = false;
            _shotting = false;
        }
    }
    /// <summary>
    /// �A�r���e�B�P���Ăяo������
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility1(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility1");
    }
    /// <summary>
    /// �A�r���e�B�Q���Ăяo������
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility2(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility2");
    }
    /// <summary>
    /// �A�r���e�B�R���Ăяo������
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility3(InputAction.CallbackContext context)
    {
        Debug.Log("UseAbility3");
    }

    public void ModeReset()
    {
        _mode = Mode.submachineGun;
        _canShotConvert = true;
    }
    public enum Mode
    {
        submachineGun,
        railgun,
    }
}

