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
    [SerializeField] GameObject _Anchor;
    [SerializeField] GameObject _playerBody;
    [SerializeField] Animator _anim;
    [SerializeField] Rigidbody _rigidbody;

    //�ʏ�ˌ��̂��߂̕ϐ�
    [SerializeField] UnityEvent _shot;
    public bool _shotting = false;
    float _shotIntervalTimer = 0.2f;
    float anchorTimer = 0;

    public Vector3 _movePower = Vector3.zero;
    public Vector2 look;

    public bool _canShotConvert = true;//�ʏ�ˌ��A�R���o�[�g�A���[���K���ˌ����Ǘ�
    public bool _canUseAbility = false;
    bool _canJump = false;
    private float _groundDistance = 0;

    //�v���C���[�̏�Ԃ�ۑ�����ϐ�
    public bool _moving = false;
    public bool _jumping = false;



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
        if (_shotting && _shotIntervalTimer <=0)
        {
            _shot.Invoke();
            _shotIntervalTimer = 1 / _weaponStatus.RateOfFire;
        }
        if (_shotIntervalTimer > 0)
        {
            _shotIntervalTimer -= Time.deltaTime;
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
        if(_groundDistance < 0.1f && _moving && _movePower != Vector3.zero)
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

    /// <summary>
    /// �O������A�j���[�V�����̕ύX���s���ۂɎg�p����B
    /// </summary>
    /// <param name="animName">�ύX����Bool�^�ϐ��̖��O�����</param>
    /// <param name="which">false�ɂ���ۂ̂�false�Ɠ���</param>
    public void AnimationChange(string animName , bool which = true)
    {
        _anim.SetBool(animName, which);
    }
    public void AncShot()
    {
        anchorTimer = _weaponStatus.HookShotTimer;
        _Anchor.GetComponent<Anchor>().AnchorShot(_weaponStatus.HookShotSpeed);
    }
}

