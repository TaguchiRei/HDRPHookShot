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

    //通常射撃のための変数
    [SerializeField] UnityEvent _shot;
    public bool _shotting = false;
    float _shotIntervalTimer = 0.2f;
    float anchorTimer = 0;

    public Vector3 _movePower = Vector3.zero;
    public Vector2 look;

    public bool _canShotConvert = true;//通常射撃、コンバート、レールガン射撃を管理
    public bool _canUseAbility = false;
    bool _canJump = false;
    private float _groundDistance = 0;

    //プレイヤーの状態を保存する変数
    public bool _moving = false;
    public bool _jumping = false;



    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    void Update()
    {
        //プレイヤーの動きを作る
        //視点操作はここで行う。
        transform.Rotate(0, look.x * _gameManager._horizontalCamera, 0);
        //上下カメラの上限と下限を設定する。また、デフォルト値が反転操作なのでX軸回転(カメラの上下方向操作)は-1をかける
        float verticalAngle = Mathf.Clamp(look.y * _gameManager._verticalCamera * -1f, -80f, 80f);
        float playerAngle = _playerBody.transform.rotation.eulerAngles.x;
        //プレイヤーの角度をEulerAnglesで取得しているのでマイナスは360からその値を引いた数になるので360で引くことで正しい値に戻す。
        if (playerAngle > 180) playerAngle -= 360;
        if (verticalAngle + playerAngle < 80 && -80 < verticalAngle + playerAngle)
        {
            _playerBody.transform.Rotate(verticalAngle, 0, 0);
        }

        //射撃プログラム
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
        //プレイヤーの動きを作る
        if(_groundDistance < 0.1f && _moving && _movePower != Vector3.zero)
        {
            Vector3 move = transform.TransformDirection(_movePower);
            _rigidbody.linearVelocity = new Vector3(move.x,_rigidbody.linearVelocity.y,move.z);
        }

        //重力を作る
        Physics.BoxCast(transform.position, new Vector3(transform.localScale.x,0.1f,transform.localScale.z), Vector3.down, out RaycastHit hit, Quaternion.identity);
        _groundDistance = hit.distance;
        //重力加速度に重力の強さをかけ、落ちる時はさらに重力を増加させる。地面に近づくとさらに強力に
        Vector3 gravity = new Vector3(0, -9.81f, 0) * _gravityScale;
        gravity = _rigidbody.linearVelocity.y < 0 ? gravity * 1.2f : gravity;
        gravity = _groundDistance < _gravityScaleChangePoint ? gravity * 2f : gravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        //接地判定のために地面との距離とベロシティを測る
        if (_groundDistance < 0.1f && _rigidbody.linearVelocity.y <= 0)
        {
            _canJump = true;
        }
        //ジャンプの処理
        if (_canJump && _jumping)
        {
            _rigidbody.AddForce(new Vector3(0, _jumpPower, 0), ForceMode.Impulse);
            _canJump = false;
        }
    }

    /// <summary>
    /// 外部からアニメーションの変更を行う際に使用する。
    /// </summary>
    /// <param name="animName">変更するBool型変数の名前を入力</param>
    /// <param name="which">falseにする際のみfalseと入力</param>
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

