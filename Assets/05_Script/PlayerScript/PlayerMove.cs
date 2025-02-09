using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    [HideInInspector] public GameManager _gameManager;
    public bool CanMoveInput = false;
    public bool CanJumpInput = false;
    [SerializeField] Vector3 _jumpPower = new(0, 100, 0);
    public bool CanUseWeaponInput = false;
    public WeaponStatus WeaponStatus;
    [SerializeField] float _gravityScale = 1;
    [SerializeField] float _gravityScaleChangePoint;
    [SerializeField] GameObject _playerBody;
    [SerializeField] GameObject _playerHead;
    [SerializeField] Animator _anim;
    [SerializeField] Rigidbody _rigidbody;
    public float SensitivityCorrection = 1;

    //�ʏ�ˌ��̂��߂̕ϐ�
    [SerializeField] GameObject _muzzlePos;
    [SerializeField] VisualEffect _bulletEffect;
    [SerializeField] UnityEvent _shot;
    public bool Shotting = false;
    float _shotIntervalTimer = 0.2f;
    [HideInInspector] public float anchorTimer = 0;
    public Func<int> BuffList;

    //�t�b�N�V���b�g�̕ϐ�
    [SerializeField] GameObject _anchorPrehab;
    [SerializeField] GameObject _anchorMuzzle;
    [SerializeField] MeshRenderer _anchorMesh;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] int _hookShotPower = 10;
    GameObject _anchorInstance;
    public bool HookShotHit = false;

    //���[���K���̂��߂̕ϐ�   
    [SerializeField] GameObject crackObj;
    [SerializeField] UnityEvent railGunShot;

    public Vector3 MovePower = Vector3.zero;
    public Vector2 Look;

    public bool CanAction = true;
    public bool CanUseAbility = false;
    bool _canJump = false;

    //�v���C���[�̃X�e�[�^�X��ۑ�����ϐ�
    float _maxHp = 100;
    float _hp = 100;
    float _maxEnergy = 100;
    float _energy = 100;
    [SerializeField] Image _hpImage;
    [SerializeField] Image _energyImage;


    //�v���C���[�̏�Ԃ�ۑ�����ϐ�
    public bool Moving = false;
    public bool Jumping = false;
    public bool OnGround = true;
    bool _usingAnchor = false;
    int _beamDmg = 10;
    public AbilitySet AbilitySetting;
    [SerializeField] Vector3 _defaultAbilitySet;

    //�A�r���e�B�p
    [SerializeField] AbilityData abilityData;

    //�ꎞ��~�����p
    Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        //�퓬�J�n���ɏ��������Ȃ���΂����Ȃ�����
        //�����X�e�[�^�X�ǂݍ���
        //�X�L���Z�b�g�ǂݍ���
        //�S��ԃ��Z�b�g
        AbilitySetting.abilityNumber1 = (int)_defaultAbilitySet.x;
        AbilitySetting.abilityNumber2 = (int)_defaultAbilitySet.y;
        AbilitySetting.abilityNumber3 = (int)_defaultAbilitySet.z;
        Debug.Log(AbilitySetting.abilityNumber1);
        Debug.Log(AbilitySetting.abilityNumber2);
        Debug.Log(AbilitySetting.abilityNumber3);
        _gameManager.InButtlePause += Pause;
        _gameManager.InButtleReStart += ReStart;
    }

    void Update()
    {
        if (!_gameManager._pause)
        {
            //�v���C���[�̓��������
            //���_����͂����ōs���B
            transform.Rotate(0, Look.x * _gameManager._horizontalCamera * SensitivityCorrection, 0);
            //�㉺�J�����̏���Ɖ�����ݒ肷��B�܂��A�f�t�H���g�l�����]����Ȃ̂�X����](�J�����̏㉺��������)��-1��������
            float verticalAngle = Mathf.Clamp(Look.y * _gameManager._verticalCamera * -1f * SensitivityCorrection, -80f, 80f);
            float playerAngle = _playerBody.transform.rotation.eulerAngles.x;
            //�v���C���[�̊p�x��EulerAngles�Ŏ擾���Ă���̂Ń}�C�i�X��360���炻�̒l�����������ɂȂ�̂�360�ň������ƂŐ������l�ɖ߂��B
            if (playerAngle > 180) playerAngle -= 360;
            if (verticalAngle + playerAngle < 80 && -80 < verticalAngle + playerAngle)
            {
                _playerBody.transform.Rotate(verticalAngle, 0, 0);
            }

            //�ˌ��v���O����
            if (Shotting && _shotIntervalTimer <= 0)
            {
                _shot.Invoke();
                var ray = Physics.Raycast(_playerHead.transform.position, _playerHead.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Enemy", "Ground", "Default"));
                _bulletEffect.SetInt("BulletType", 0);
                _bulletEffect.SetVector3("StartPos", _muzzlePos.transform.position);
                _bulletEffect.SetVector3("EndVector", hit.point - _muzzlePos.transform.position);
                _bulletEffect.SendEvent("NormalBullet");
                _shotIntervalTimer = 1 / WeaponStatus.RateOfFire;
                if (ray)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        //�����Ɏˌ��������������̏����������B
                        hit.collider.gameObject.GetComponent<EnemyStatus>().HPChanger(2);
                        GaugeChanger(-1, false);
                    }
                    else if (hit.collider.CompareTag("Barrier"))
                    {
                        hit.collider.gameObject.GetComponent<DefenderEnemyShield>().HPChanger(1);
                    }
                    else if (hit.collider.CompareTag("Target"))
                    {
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
            if (_shotIntervalTimer > 0)
            {
                _shotIntervalTimer -= Time.deltaTime;
            }
            if (!HookShotHit && anchorTimer >= 0)
            {
                anchorTimer -= Time.deltaTime;
                if (anchorTimer < 0)
                {
                    HookShotHit = false;
                    AncDestroy();
                    _anchorInstance = null;
                    _anim.SetBool("HookShotOrAim", false);
                    CanAction = true;
                    _anchorMesh.enabled = true;
                }
            }
            if (_usingAnchor)
            {
                _lineRenderer.SetPosition(0, _anchorMuzzle.transform.position);
                _lineRenderer.SetPosition(1, _anchorInstance.transform.position);
            }
            //�n�ʂƂ̋����𑪂�{�b�N�X�L���X�g
            bool groundHit = Physics.BoxCast(new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z), new Vector3(0.5f, 0.5f, 0.5f), Vector3.down, Quaternion.identity, 0.8f);
            if (groundHit) OnGround = true;
            else OnGround = false;
        }
    }
    private void FixedUpdate()
    {
        _beamDmg++;
        if (!_gameManager._pause)
        {
            //�v���C���[�̓��������
            if (Moving && MovePower != Vector3.zero)
            {
                if (!HookShotHit && OnGround)
                {
                    Vector3 move = transform.TransformDirection(MovePower);
                    _rigidbody.linearVelocity = new Vector3(move.x, _rigidbody.linearVelocity.y, move.z);
                }
                else
                {
                    Vector3 move = transform.TransformDirection(MovePower);
                    _rigidbody.AddForce(move * 3, ForceMode.Acceleration);
                }
            }

            //�d�͉����x�ɏd�͂̋�����������B
            Vector3 gravity = _rigidbody.linearVelocity.y <= 0 ? new Vector3(0, -20, 0) * _gravityScale : new Vector3(0, -9.81f, 0) * _gravityScale;
            _rigidbody.AddForce(gravity, ForceMode.Acceleration);
            //�ڒn����̂��߂ɒn�ʂƂ̋����ƃx���V�e�B�𑪂�
            if (OnGround && _rigidbody.linearVelocity.y <= 0)
            {
                _canJump = true;
            }
            //�W�����v�̏���
            if (Jumping)
            {
                if (_canJump)
                {
                    _rigidbody.AddForce(_jumpPower, ForceMode.Impulse);
                    _canJump = false;
                }
                else if (HookShotHit)
                {
                    _rigidbody.AddForce(_jumpPower / 10);
                }
            }
            //�t�b�N�V���b�g���h�����Ă���Ƃ��̏���
            if (HookShotHit)
            {
                Vector3 hookPower = _anchorInstance.transform.position - transform.position;
                hookPower.Normalize();
                hookPower *= 9.81f * _hookShotPower;
                _rigidbody.AddForce(hookPower, ForceMode.Acceleration);
            }
        }
    }

    public IEnumerator ShotRailGun()
    {
        railGunShot.Invoke();
        var railgunHit = Physics.SphereCastAll(_playerHead.transform.position, 2f, _playerHead.transform.forward, Mathf.Infinity, LayerMask.GetMask("Enemy", "Ground", "Default"));
        if (railgunHit != null)
        {
            foreach (var ray in railgunHit)
            {
                if (ray.collider.CompareTag("Enemy"))
                {
                    ray.collider.gameObject.GetComponent<EnemyStatus>().HPChanger(2);
                }
                else if (ray.collider.CompareTag("Barrier"))
                {
                    ray.collider.gameObject.GetComponent<DefenderEnemyShield>().HPChanger(2);
                    break;
                }
            }
            GaugeChanger(railgunHit.Length * -5);
        }
        var crackObjInstance = Instantiate(crackObj, _anchorMuzzle.transform.position, _anchorMuzzle.transform.rotation);
        yield return new WaitForSeconds(5f);
        Destroy(crackObjInstance);
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
        anchorTimer = WeaponStatus.HookShotTimer;
        _anchorMesh.enabled = false;
        _anchorInstance = Instantiate(_anchorPrehab, _playerHead.transform.position, _playerHead.transform.rotation);
        var anc = _anchorInstance.GetComponent<Anchor>();
        anc._moveDirection += _rigidbody.linearVelocity * 0.1f;
        anc._playerMove = this;
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
        var useEnergy = abilityData.abilityData[abilityNumber].abilityCost;
        if (useEnergy <= _energy)
        {
            GaugeChanger(useEnergy, false);
            _anim.SetBool("UseAbility", true);
            _anim.SetInteger("AbilityNumber", abilityNumber);
        }
        else
        {
            CanAction = true;
        }
    }

    /// <summary>
    /// �v���C���[�̃Q�[�W��ύX������B
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="hpChange">false�ɂ���ƃG�l���M�[�Q�[�W��ύX�ł���</param>
    public void GaugeChanger(float amount, bool hpChange = true)
    {
        if (hpChange)
        {
            _hp -= amount;
            if (_hp < 0)
                _hp = 0;
            else if (_hp > _maxHp)
                _hp = _maxHp;

            _hpImage.DOFillAmount(_hp / _maxHp, 0.1f);
        }
        else
        {
            _energy -= amount;
            if (_energy < 0)
                _energy = 0;
            else if (_energy > _maxEnergy)
                _energy = _maxEnergy;

            _energyImage.DOFillAmount(_energy / _maxEnergy, 0.1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DMGZone") && _beamDmg > 5)
        {
            GaugeChanger(40);
            _beamDmg = 0;
        }
    }
    public struct AbilitySet
    {
        public int abilityNumber1;
        public int abilityNumber2;
        public int abilityNumber3;
    }
}

