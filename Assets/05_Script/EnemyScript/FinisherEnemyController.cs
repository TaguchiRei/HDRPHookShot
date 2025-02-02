using UnityEngine;

public class FinisherEnemyController : EnemyBase
{
    [SerializeField] GameObject _dangerZone;
    [SerializeField] GameObject _beamObject;
    bool beamStart = false;
    Collider[] colliders;
    GameObject feed;
    public override void UniqueAction(Vector3 delayedPosition)
    {
        colliders = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Enemy"));
        if (beamStart || colliders.Length == 0)
            return;

        beamStart = true;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<EnemyStatus>().EnemyType == EnemyType.attacker)
            {
                feed = collider.gameObject;
                break;
            }
        }
        var feedController = feed.GetComponent<AttackerEnemyController>();
        feedController.Stop();
        feedController.CanMove = false;
    }

    public override void Move(Vector3 position)
    {
        if (!beamStart)
        {
            base.Move(position);
        }
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
}
