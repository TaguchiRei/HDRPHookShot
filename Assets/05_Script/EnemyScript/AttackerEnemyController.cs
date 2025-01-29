using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AttackerEnemyController : EnemyBase
{

    [SerializeField] GameObject _eye;

    Queue<Vector3> positionHistory = new();
    float elapsedTime = 1;
    [SerializeField] float delay = 1f;
    [SerializeField] Transform _muzzlePos;
    float shotInterval;
    Vector3 delayedPosition = Vector3.zero;
    Vector3 direction;
    List<AttackerEnemyController> attackerEnemyControllerList = new();
    VisualEffect _bulletEfect;

    private void Start()
    {
        PlayerHead = GameObject.FindGameObjectWithTag("PlayerHead");
        _bulletEfect = GameObject.FindGameObjectWithTag("VFX").GetComponent<VisualEffect>();
        timer = 5;
    }
    void Update()
    {
        if (survive)
        {
            Physics.Raycast(transform.position, (PlayerHead.transform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground"));

            if (Agent.remainingDistance <= 0.5f && !Agent.hasPath)
            {
                Animator.SetBool("Walking", false);
            }
            else
            {
                Animator.SetBool("Walking", true);
            }

            if (enemyStatus.Leader)
            {
                LeaderMove(hit);

                positionHistory.Enqueue(PlayerHead.transform.position);
                elapsedTime += Time.deltaTime;
                shotInterval -= Time.deltaTime;

                if (elapsedTime >= delay)
                {
                    delayedPosition = positionHistory.Dequeue();
                    if (shotInterval < 0)
                    {
                        shotInterval = Random.Range(0.5f, 1.0f);
                        Shot(delayedPosition);
                        if (attackerEnemyControllerList.Count < enemyStatus.MembersList.Count)
                        {
                            UniqueInitialization();
                        }
                        foreach (var attacker in attackerEnemyControllerList)
                        {
                            attacker.Shot(delayedPosition);
                        }
                    }
                }
            }
        }
    }
    public override void UniqueInitialization()
    {
        attackerEnemyControllerList.Clear();
        foreach (var member in enemyStatus.MembersList)
        {
            attackerEnemyControllerList.Add(member.GetComponent<AttackerEnemyController>());
        }
    }

    public void Shot(Vector3 aimPos)
    {
        Physics.Raycast(transform.position, (PlayerHead.transform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground"));
        _bulletEfect.SetInt("BulletType", 1);
        _bulletEfect.SetVector3("StartPos", _muzzlePos.transform.position);
        _bulletEfect.SetVector3("EndVector", (PlayerHead.transform.forward * 10000) - _muzzlePos.transform.forward);
        _bulletEfect.SendEvent("NomalBullet");
    }

    public override void Move(Vector3 position)
    {
        base.Move(position);
        Animator.SetBool("Walking", true);
    }
}
