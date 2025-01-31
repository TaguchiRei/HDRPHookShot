using UnityEngine;

public class DefenderEnemyController : EnemyBase
{
    bool garding = false;
    Vector3 movePoint = Vector3.zero;
    
    public override void UniqueAction(Vector3 delayedPosition)
    {
        Animator.SetBool("", true);
    }

    public override void UniqueInitialization()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
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
