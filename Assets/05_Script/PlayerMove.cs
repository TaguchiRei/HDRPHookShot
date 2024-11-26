using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    GameManager _gameManager;
    public bool _canMoveInput = false;
    public bool _canJumpInput = false;
    [SerializeField] float _jumpPower = 1;
    public bool _canUseWeaponInput = false;
    public WeaponStatus _weaponStatus;
    [SerializeField] float _gravityScale = 1;
    [SerializeField] float _gravityScaleChangePoint;
    [SerializeField] GameObject _bullet;
    [SerializeField] GameObject _playerBody;
    [SerializeField] GameObject _playerHead;
    [SerializeField] Animator _anim;
    [SerializeField] Rigidbody _rigidbody;

    //�ʏ�ˌ��̂��߂̕ϐ�
    [SerializeField] UnityEvent _shot;
    public bool _shotting = false;
    float _shotIntervalTimer = 0.2f;
    float anchorTimer = 0;

    //�t�b�N�V���b�g�̕ϐ�
    [SerializeField] GameObject _AnchorPrehab;
    [SerializeField] GameObject _AnchorMuzzle;
    [SerializeField] MeshRenderer _anchorMesh;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] int _hookShotPower = 10;
    GameObject _anchorInstance;
    public bool _hookShotHit = false;
    bool boost = false;


    public Vector3 _movePower = Vector3.zero;
    public Vector2 look;

    public bool _canShotConvert = true;
    public bool _canUseAbility = false;
    bool _canJump = false;

    //�v���C���[�̏�Ԃ�ۑ�����ϐ�
    public bool _moving = false;
    public bool _jumping = false;
    bool _usingAnchor = false;
    bool _onGround = true;
    RaycastHit hit;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
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
        if (_shotting && _shotIntervalTimer <= 0)
        {
            _shot.Invoke();
            _shotIntervalTimer = 1 / _weaponStatus.RateOfFire;
        }
        if (_shotIntervalTimer > 0)
        {
            _shotIntervalTimer -= Time.deltaTime;
        }
        if (anchorTimer > 0 && !_hookShotHit)
        {
            anchorTimer -= Time.deltaTime;
            if (anchorTimer < 0)
            {
                _hookShotHit = false;
                AncDestroy();
                _anchorInstance = null;
                _anim.SetBool("HookShotOrAim", false);
                _canShotConvert = true;
                _anchorMesh.enabled = true;
            }
        }
        if (_usingAnchor)
        {
            _lineRenderer.SetPosition(0, _AnchorMuzzle.transform.position);
            _lineRenderer.SetPosition(1, _anchorInstance.transform.position);
        }
        //�n�ʂƂ̋����𑪂�{�b�N�X�L���X�g
        bool groundHit = Physics.BoxCast(
            new Vector3(transform.position.x, transform.position.y+0.6f, transform.position.z),
            new Vector3(0.5f, 0.5f, 0.5f),
            Vector3.down,
            Quaternion.identity,
            0.8f);
        if (groundHit)
        {
            _onGround = true;
            Debug.Log("OnGround");
        }
        else
        {
            _onGround = false;
            Debug.Log("NotGround");
        }
    }
    private void FixedUpdate()
    {
        //�v���C���[�̓��������
        if (_moving && _movePower != Vector3.zero)
        {
            if (!_hookShotHit && _onGround)
            {
                Vector3 move = transform.TransformDirection(_movePower);
                _rigidbody.linearVelocity = new Vector3(move.x, _rigidbody.linearVelocity.y, move.z);
            }
            else
            {
                Vector3 move = transform.TransformDirection(_movePower);
                _rigidbody.AddForce(move, ForceMode.Acceleration);
            }
        }

        //�d�͉����x�ɏd�͂̋�����������B
        Vector3 gravity = _rigidbody.linearVelocity.y <= 0 ? new Vector3(0, -20, 0) * _gravityScale : new Vector3(0, -9.81f, 0) * _gravityScale;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        //�ڒn����̂��߂ɒn�ʂƂ̋����ƃx���V�e�B�𑪂�
        if (_onGround && _rigidbody.linearVelocity.y <= 0)
        {
            _canJump = true;
        }
        //�W�����v�̏���
        if (_canJump && _jumping)
        {
            _rigidbody.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
            _canJump = false;
        }
        //�t�b�N�V���b�g���h�����Ă���Ƃ��̏���
        if (_hookShotHit)
        {
            Vector3 hookPower = _anchorInstance.transform.position - transform.position;
            hookPower.Normalize();
            hookPower *= 9.81f * _hookShotPower;
            _rigidbody.AddForce(hookPower, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// �O������A�j���[�V�����̕ύX���s���ۂɎg�p����B
    /// </summary>
    /// <param name="animName">�ύX����Bool�^�ϐ��̖��O�����</param>
    /// <param name="which">false�ɂ���ۂ̂�false�Ɠ���</param>
    public void AnimationChange(string animName, bool which = true)
    {
        _anim.SetBool(animName, which);
    }
    public void AncShot()
    {
        _lineRenderer.enabled = true;
        _usingAnchor = true;
        anchorTimer = _weaponStatus.HookShotTimer;
        _anchorMesh.enabled = false;
        _anchorInstance = Instantiate(_AnchorPrehab, _playerHead.transform.position, _playerHead.transform.rotation);
        _anchorInstance.GetComponent<Anchor>()._moveDirection += _rigidbody.linearVelocity * 0.1f;
    }
    public void AncDestroy()
    {
        Destroy(_anchorInstance);
        _anchorInstance = null;
        _anchorMesh.enabled = true;
        _usingAnchor = false;
        _lineRenderer.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + new Vector3(0, 0.5f, 0);
        Quaternion rotation = Quaternion.identity;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(origin, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f));
    }
}

