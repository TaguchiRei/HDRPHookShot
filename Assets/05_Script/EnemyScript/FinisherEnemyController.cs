using UnityEngine;

public class FinisherEnemyController : EnemyBase
{
    bool _beamStart = false;
    float _timer = 0;
    float _moveTimer = 0;
    [SerializeField] float _maxTimer = 4;
    float _recastTime = 0;
    [SerializeField] float _defaultRecastTime = 10;
    Collider[] _colliders;
    GameObject _feed;
    int beamPhase = 0;
    [SerializeField] GameObject _mainBody;
    [SerializeField] GameObject _beamObject;
    [SerializeField] Animator _beamAnimator;
    [SerializeField] GameObject _dangerAreaObject;
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
                    if (_timer < 1f)
                    {
                        _mainBody.transform.LookAt(PlayerHead.transform.position);
                    }
                    else if (_timer < 0f)
                    {
                        beamPhase = 3;
                    }
                    _dangerAreaObject.SetActive(true);
                    break;
                case 3:
                    _dangerAreaObject.SetActive(false);
                    _beamObject.SetActive(true);
                    _beamAnimator.SetBool("beam", true);
                    beamPhase = 4;
                    _moveTimer = 1f;
                    break;
                case 4:
                    if(_timer < 0f)
                    {
                        beamPhase = 0;
                        _dangerAreaObject.transform.rotation = Quaternion.identity;
                        _beamStart = false;
                        Animator.SetBool("grab", false);
                    }
                    else
                    {
                        _timer -= Time.deltaTime;
                    }
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
        EnemyStatus.Invincible = true;
        _timer = _maxTimer;
    }
}
