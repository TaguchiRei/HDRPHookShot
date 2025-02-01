using GamesKeystoneFramework.PolarCoordinates;
using UnityEngine;

public class DefenderEnemyController : EnemyBase
{
    [SerializeField] GameObject ShieldObjedt;
    [SerializeField] DefenderEnemyShield DefenderEnemyShield;
    float _recastTimer = 0;
    [SerializeField] float _moveSpeedChange = 30f;
    [SerializeField] float _recastTime = 10;
    float defaultMoveSpeed = 0;
    bool _guard = false;
    public override void UniqueAction(Vector3 delayedPosition)
    {
        Animator.SetBool("Defense", true);
        DefenderEnemyShield.SummonShield();
        _guard = true;
    }

    public override void UniqueInitialization()
    {

    }

    public override void Start()
    {
        base.Start();
        EnemyStatus.Initialization(Random.Range(20,50),LR.right,true,new(),gameObject);
    }

    public override void Update()
    {
        base.Update();
        if (_guard)
        {
            ShieldObjedt.transform.LookAt(delayedPosition);
        }
    }
    public void BreakShield()
    {
        _recastTimer = _recastTime;
        defaultMoveSpeed = MoveSpeed;
        MoveSpeed = _moveSpeedChange;
        Move(Vector3.zero);
    }
    public override void Move(Vector3 position)
    {
        if (_recastTimer < 0)
        {
            base.Move(position);
            DefenderEnemyShield.TakeAwayShield();
            Animator.SetBool("Defense", false);
            Animator.SetBool("Walking", true);
        }
        else
        {
            PolarCoordinates polarCoordinates = PolarCoordinatesSupport.ToPolarCoordinates(new Vector2(PlayerMoveI.transform.position.x, PlayerMoveI.transform.position.z) - new Vector2(transform.position.x, transform.position.z));
            polarCoordinates = new(polarCoordinates.radius * 2f, polarCoordinates.angle + Random.Range(-0.5f, 0.6f));
            base.Move(transform.position + new Vector3(polarCoordinates.ToVector2().x,0,polarCoordinates.ToVector2().y));
        }
    }
}
