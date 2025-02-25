using UnityEngine;

public class DefenderEnemyController : EnemyBase
{
    [SerializeField] GameObject ShieldObject;
    [SerializeField] DefenderEnemyShield DefenderEnemyShield;
    float _recastTimer = 0;
    [SerializeField] float _recastTime = 10;
    bool _guard = false;
    public override void UniqueAction(Vector3 delayedPosition)
    {
        if (Agent.velocity.magnitude < 0.5f)
        {
            if (_recastTimer <= 0)
            {
                if (!Animator.GetBool("Walking"))
                {
                    Animator.SetBool("Defense", true);
                }
                DefenderEnemyShield.SummonShield();
                _guard = true;
            }
        }
    }

    public override void UniqueInitialization()
    {

    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        if (_guard)
        {
            ShieldObject.transform.LookAt(PlayerHead.transform.position);
        }
        if (_recastTimer > 0)
        {
            _recastTimer -= Time.deltaTime;

        }
    }
    public void BreakShield()
    {
        _recastTimer = _recastTime;
    }
    public void ShieldOpened()
    {
        Animator.SetBool("Defense",false);
    }
    public override void Move(Vector3 position)
    {
        base.Move(position);
        DefenderEnemyShield.TakeAwayShield();
        Animator.SetBool("Defense", false);
        Animator.SetBool("Walking", true);
    }
}
