using UnityEngine;

public class DefenderEnemyController : EnemyBase
{
    bool garding = false;
    Vector3 movePoint = Vector3.zero;
    [SerializeField] GameObject ShieldObject;
    public override void UniqueAction(Vector3 delayedPosition)
    {
        Animator.SetBool("", true);
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
    }
    public override void Move(Vector3 position)
    {
        if (!garding)
        {
            base.Move(position);
            Animator.SetBool("Walking", true);
        }
        else
        {
            movePoint = position;
        }
    }
}
