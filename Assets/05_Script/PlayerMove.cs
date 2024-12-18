using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    public GameManager _gameManager;
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
    [SerializeField] GameObject bullet;
    public bool _shotting = false;
    float _shotIntervalTimer = 0.2f;
    float anchorTimer = 0;
    public Func<int> _buffList;

    //�t�b�N�V���b�g�̕ϐ�
    [SerializeField] GameObject _AnchorPrehab;
    [SerializeField] GameObject _AnchorMuzzle;
    [SerializeField] MeshRenderer _anchorMesh;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] int _hookShotPower = 10;
    GameObject _anchorInstance;
    public bool _hookShotHit = false;


    public Vector3 _movePower = Vector3.zero;
    public Vector2 look;

    public bool _canAction = true;
    public bool _canUseAbility = false;
    bool _canJump = false;

    //�v���C���[�̃X�e�[�^�X��ۑ�����ϐ�
    float _maxHp = 100;
    float _hp = 100;
    float _maxEnergy = 100;
    float _energy = 100;
    [SerializeField] Image _hpImage;
    [SerializeField] Image _energyImage;


    //�v���C���[�̏�Ԃ�ۑ�����ϐ�
    public bool _moving = false;
    public bool _jumping = false;
    public bool _onGround = true;
    bool _usingAnchor = false;
    bool _usingAbility = false;
    public AbilitySet _abilitySet;
    RaycastHit hit;
    [SerializeField] Vector3 _defaultAbilitySet;

    //�ꎞ��~�����p
    Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        //�퓬�J�n���ɏ��������Ȃ���΂����Ȃ�����
        //�����X�e�[�^�X�ǂݍ���
        //�X�L���Z�b�g�ǂݍ���
        //�S��ԃ��Z�b�g
        _abilitySet.abilityNumber1 = (int)_defaultAbilitySet.x;
        _abilitySet.abilityNumber2 = (int)_defaultAbilitySet.y;
        _abilitySet.abilityNumber3 = (int)_defaultAbilitySet.z;
        Debug.Log(_abilitySet.abilityNumber1);
        Debug.Log(_abilitySet.abilityNumber2);
        Debug.Log(_abilitySet.abilityNumber3);
        _gameManager.InButtlePause += Pause;
        _gameManager.InButtleReStart += ReStart;
    }

    void Update()
    {
        if (!_gameManager._pause)
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
                    _canAction = true;
                    _anchorMesh.enabled = true;
                }
            }
            if (_usingAnchor)
            {
                _lineRenderer.SetPosition(0, _AnchorMuzzle.transform.position);
                _lineRenderer.SetPosition(1, _anchorInstance.transform.position);
            }
            //�n�ʂƂ̋����𑪂�{�b�N�X�L���X�g
            bool groundHit = Physics.BoxCast(new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z), new Vector3(0.5f, 0.5f, 0.5f), Vector3.down, Quaternion.identity, 0.8f);
            if (groundHit) _onGround = true;
            else _onGround = false;
        }
    }
    private void FixedUpdate()
    {
        if (!_gameManager._pause)
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
    }

    void Pause()
    {
        _anim.speed = 0;
        _velocity = _rigidbody.linearVelocity;
        _rigidbody.linearVelocity = Vector3.zero;
    }
    void ReStart()
    {
        _anim.speed = 1;
        _rigidbody.linearVelocity = _velocity;
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
    public void UseAbility(int abilityNumber)
    {
        _anim.SetBool("UseAbility", true);
        _anim.SetInteger("AbilityNumber", abilityNumber);
    }

    public void GaugeChanger(float amount, bool hpChange = true)
    {
        if (hpChange)
        {
            _hp -= amount;
            if (_hp < 0) _hp = 0;
            else if (_hp > _maxHp) _hp = _maxHp;
            _hpImage.DOFillAmount(_hp / _maxHp, 0.1f);
        }
        else
        {
            _energy -= amount;
            if (_energy < 0) _energy = 0;
            else if (_energy > _maxEnergy) _energy = _maxEnergy;
            _energyImage.DOFillAmount(_energy / _maxEnergy, amount);
        }
    }

    public struct AbilitySet
    {
        public int abilityNumber1;
        public int abilityNumber2;
        public int abilityNumber3;
    }
}

