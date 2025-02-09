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

    //通常射撃のための変数
    [SerializeField] GameObject _muzzlePos;
    [SerializeField] VisualEffect _bulletEffect;
    [SerializeField] UnityEvent _shot;
    public bool Shotting = false;
    float _shotIntervalTimer = 0.2f;
    [HideInInspector] public float anchorTimer = 0;
    public Func<int> BuffList;

    //フックショットの変数
    [SerializeField] GameObject _anchorPrehab;
    [SerializeField] GameObject _anchorMuzzle;
    [SerializeField] MeshRenderer _anchorMesh;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] int _hookShotPower = 10;
    GameObject _anchorInstance;
    public bool HookShotHit = false;

    //レールガンのための変数   
    [SerializeField] GameObject crackObj;
    [SerializeField] UnityEvent railGunShot;

    public Vector3 MovePower = Vector3.zero;
    public Vector2 Look;

    public bool CanAction = true;
    public bool CanUseAbility = false;
    bool _canJump = false;

    //プレイヤーのステータスを保存する変数
    float _maxHp = 100;
    float _hp = 100;
    float _maxEnergy = 100;
    float _energy = 100;
    [SerializeField] Image _hpImage;
    [SerializeField] Image _energyImage;


    //プレイヤーの状態を保存する変数
    public bool Moving = false;
    public bool Jumping = false;
    public bool OnGround = true;
    bool _usingAnchor = false;
    int _beamDmg = 10;
    public AbilitySet AbilitySetting;
    [SerializeField] Vector3 _defaultAbilitySet;

    //アビリティ用
    [SerializeField] AbilityData abilityData;

    //一時停止処理用
    Vector3 _velocity = Vector3.zero;

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        //戦闘開始時に初期化しなければいけないもの
        //初期ステータス読み込み
        //スキルセット読み込み
        //全状態リセット
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
            //プレイヤーの動きを作る
            //視点操作はここで行う。
            transform.Rotate(0, Look.x * _gameManager._horizontalCamera * SensitivityCorrection, 0);
            //上下カメラの上限と下限を設定する。また、デフォルト値が反転操作なのでX軸回転(カメラの上下方向操作)は-1をかける
            float verticalAngle = Mathf.Clamp(Look.y * _gameManager._verticalCamera * -1f * SensitivityCorrection, -80f, 80f);
            float playerAngle = _playerBody.transform.rotation.eulerAngles.x;
            //プレイヤーの角度をEulerAnglesで取得しているのでマイナスは360からその値を引いた数になるので360で引くことで正しい値に戻す。
            if (playerAngle > 180) playerAngle -= 360;
            if (verticalAngle + playerAngle < 80 && -80 < verticalAngle + playerAngle)
            {
                _playerBody.transform.Rotate(verticalAngle, 0, 0);
            }

            //射撃プログラム
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
                        //ここに射撃が当たった時の処理を書く。
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
            //地面との距離を測るボックスキャスト
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
            //プレイヤーの動きを作る
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

            //重力加速度に重力の強さをかける。
            Vector3 gravity = _rigidbody.linearVelocity.y <= 0 ? new Vector3(0, -20, 0) * _gravityScale : new Vector3(0, -9.81f, 0) * _gravityScale;
            _rigidbody.AddForce(gravity, ForceMode.Acceleration);
            //接地判定のために地面との距離とベロシティを測る
            if (OnGround && _rigidbody.linearVelocity.y <= 0)
            {
                _canJump = true;
            }
            //ジャンプの処理
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
            //フックショットが刺さっているときの処理
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
    /// 外部からアニメーションの変更を行う際に使用する。
    /// </summary>
    /// <param name="animName">変更するBool型変数の名前を入力</param>
    /// <param name="which">falseにする際のみfalseと入力</param>
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
    /// プレイヤーのゲージを変更させる。
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="hpChange">falseにするとエネルギーゲージを変更できる</param>
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

