using UnityEngine;

public class FinisherEnemyController : EnemyBase
{
    bool _beamStart = false;
    float _recastTime = 0;
    [SerializeField] float _defaultRecastTime = 10;
    Collider[] _colliders;
    GameObject _feed;
    int beamPhase = 0;
    [SerializeField] GameObject _mainBody;
    public override void UniqueAction(Vector3 delayedPosition)
    {
        _colliders = Physics.OverlapSphere(transform.position, 20, LayerMask.GetMask("Enemy"));
        if (_beamStart || _colliders.Length == 0)
            return;

        _beamStart = true;
        foreach (Collider collider in _colliders)
        {
            if (collider.gameObject.GetComponent<EnemyStatus>().EnemyType == EnemyType.attacker)
            {
                _feed = collider.gameObject;
                break;
            }
        }
        var feedController = _feed.GetComponent<AttackerEnemyController>();
        feedController.Stop();
        feedController.CanMove = false;
        feedController.Animator.enabled = false;
        Animator.SetBool("", true);
        Agent.SetDestination(_feed.transform.position);
        beamPhase = 0;
    }

    public override void Move(Vector3 position)
    {
        if (!_beamStart)
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
        if (_beamStart)
        {
            switch (beamPhase)
            {
                case 0:
                    if (Vector3.Distance(Agent.transform.position, Agent.destination) < 0.5f || Agent.isStopped)
                    {
                        beamPhase++;
                        Animator.SetBool("grab", true);
                    }
                    break;
                case 2:

                    break;
                default:
                    break;
            }
        }
    }
    public void AttackForEat()
    {
        beamPhase++;
        _feed.GetComponent<EnemyStatus>().HPChanger(100);
    }
    public void AimStart()
    {
        beamPhase = 2;
    }
}
