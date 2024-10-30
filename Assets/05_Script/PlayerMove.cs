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

        //�A�N�V�����C�x���g��o�^

        // Move�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Move.started += OnMove;
        _playerInput.Player.Move.canceled += OnMove;

        // Look�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Look.performed += OnLook;
        _playerInput.Player.Look.started += OnLook;
        _playerInput.Player.Look.canceled += OnLook;

        // Jump�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Jump.performed += OnJump;
        _playerInput.Player.Jump.started += OnJump;
        _playerInput.Player.Jump.canceled += OnJump;

        // Shot�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Shot.performed += OnShot;
        _playerInput.Player.Shot.started += OnShot;
        _playerInput.Player.Shot.canceled += OnShot;

        // Interact�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Interact.performed += OnInteract;
        _playerInput.Player.Interact.started += OnInteract;
        _playerInput.Player.Interact.canceled += OnInteract;

        // Aim�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Aim.performed += OnAim;
        _playerInput.Player.Aim.started += OnAim;
        _playerInput.Player.Aim.canceled += OnAim;

        // Convert�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Convert.performed += OnConvert;
        _playerInput.Player.Convert.started += OnConvert;
        _playerInput.Player.Convert.canceled += OnConvert;

        // Ability1�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Ability1.performed += OnAbility1;
        _playerInput.Player.Ability1.started += OnAbility1;
        _playerInput.Player.Ability1.canceled += OnAbility1;

        // Ability2�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Ability2.performed += OnAbility2;
        _playerInput.Player.Ability2.started += OnAbility2;
        _playerInput.Player.Ability2.canceled += OnAbility2;

        // Ability3�A�N�V�����̃C�x���g�o�^
        _playerInput.Player.Ability3.performed += OnAbility3;
        _playerInput.Player.Ability3.started += OnAbility3;
        _playerInput.Player.Ability3.canceled += OnAbility3;

        // �A�N�V������L����
        _playerInput.Enable();
    }

    void Update()
    {
        //�v���C���[�̓��������

    }
    private void FixedUpdate()
    {
        //�v���C���[�̓��������
        _rigidbody.AddForce(_movePower);
        //�d�͂����
        Physics.BoxCast(transform.position, transform.localScale / 2, Vector3.down, out RaycastHit hit, Quaternion.identity);
        _groundDistanse = hit.distance;
        //�d�͉����x�ɏd�͂̋����������A�����鎞�͂���ɏd�͂𑝉�������B�n�ʂɋ߂Â��Ƃ���ɋ��͂�
        Vector3 gravity = new Vector3(0, -9.81f, 0) * _gravityScale;
        gravity = _rigidbody.linearVelocity.y < 0 ? gravity * 1.2f : gravity;
        gravity = _groundDistanse < _gravityScaleChangePoint ? gravity * 2f : gravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnDestroy()
    {
        _playerInput?.Dispose();
    }


    /* ���������͓��͎�t�̂��߂̃v���O����*/

    /// <summary>
    /// �ړ��̂��߂̏����������BmovePower�̒l�������ŕύX�B
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>() * _moveSpeed;
        _movePower = new Vector3(vector2.x, 0, vector2.y);
    }
    /// <summary>
    /// ���_����̂��߂̃v���O�����������B
    /// </summary>
    /// <param name="context"></param>
    private void OnLook(InputAction.CallbackContext context)
    {
        //��]���x�𒲐߂��邽�߂�20�Ŋ����Ă���B�܂��A�f�t�H���g�l�����]����Ȃ̂�X����]��-1��������
        Vector2 look = context.ReadValue<Vector2>() / 20f;
        transform.Rotate(0,look.x * _gameManager._horiaontalCamera,0);
        _playerBody.transform.Rotate(look.y * _gameManager._verticalCamera * -1f, 0, 0);
    }
    /// <summary>
    /// �W�����v�̂��߂̑���������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// �ˌ��̂��߂̑���������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnShot(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// �C���^���N�g�̑���������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnInteract(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// �G�C���{�^�������������̑���������ɏ����B�G�C���̏����ƃt�b�N�V���b�g���΂������B
    /// </summary>
    /// <param name="context"></param>
    private void OnAim(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// ����̕ό`�̂��߂̏����������ɏ����B
    /// </summary>
    /// <param name="context"></param>
    private void OnConvert(InputAction.CallbackContext context)
    {

    }
    /// <summary>
    /// �A�r���e�B�P���Ăяo������
    /// </summary>
    /// <param name="context"></param>
    private void OnAbility1(InputAction.CallbackContext context)
    {

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
}
