using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStatus : MonoBehaviour, IEnemyInterface
{
    public int GroupNumber { get; set; }
    public LR LR { get; set; }
    public bool Leader { get; set; }

    [SerializeField] NavMeshAgent agent;
    [SerializeField] EnemyBase enemyBase;
    [SerializeField] int _maxHp;
    public EnemyType EnemyType;
    public List<GameObject> MembersList;
    private GameObject managerObject;
    private GameObject leaderObject;
    private int Hp = 3;
    bool survive = true;
    [HideInInspector] public List<EnemyBase> EnemyBaseList = new();
    [HideInInspector] public bool Invincible = false;

    public void Initialization(int groupNumber, LR lr, bool isLeader, GameObject manager, GameObject leaderObj)
    {
        Hp = _maxHp;
        GroupNumber = groupNumber;
        LR = lr;
        Leader = isLeader;
        enemyBase.Leader = isLeader;
        managerObject = manager;
        leaderObject = leaderObj;
        survive = true;
        enemyBase.Animator.SetBool("Delete", false);
        agent.enabled = true;
        enemyBase.enabled = true;
        enemyBase.Animator.enabled = true;
        enemyBase.Survive = true;
        enemyBase.CanMove = true;
        if (!Leader)
        {
            EnemyStatus enemyStatus = leaderObject.GetComponent<EnemyStatus>();
            enemyStatus.MembersList.Add(gameObject);
            enemyStatus.EnemyBaseList.Add(enemyBase);
            enemyBase.Move(leaderObject.transform.position);
        }
        enemyBase.UniqueInitialization();
    }

    public void HPChanger(int changeNum)
    {
        if (survive && !Invincible)
        {
            Hp -= changeNum;
            if (Hp <= 0)
            {
                survive = false;
                if (!Leader)
                    leaderObject.GetComponent<EnemyStatus>().MembersList.Remove(gameObject);
                //éÄñSéûèàóù
                enemyBase.Survive = false;
                agent.enabled = false;
                enemyBase.enabled = false;
                enemyBase.Animator.SetBool("Delete", true);
                managerObject.GetComponent<EnemyManager>().EnemySpawn(GroupNumber, LR, Leader, leaderObject, MembersList);
            }
            else if (Hp > _maxHp)
            {
                Hp = _maxHp;
            }
        }
    }
    public void HookShotHit()
    {
        survive = false;
        if (!Leader)
            leaderObject.GetComponent<EnemyStatus>().MembersList.Remove(gameObject);
        //éÄñSéûèàóù
        enemyBase.Survive = false;
        agent.enabled = false;
        enemyBase.enabled = false;
        managerObject.GetComponent<EnemyManager>().EnemySpawn(GroupNumber, LR, Leader, leaderObject, MembersList);
        DeletedEnemy();
    }

    public void DeletedEnemy()
    {
        var managerC = managerObject.GetComponent<EnemyManager>();
        switch (EnemyType)
        {
            case EnemyType.attacker:
                managerC.AttackerQueue.Enqueue(gameObject);
                break;
            case EnemyType.defender:
                managerC.DefenderQueue.Enqueue(gameObject);
                break;
            default:
                managerC.SupporterQueue.Enqueue(gameObject);
                break;
        }
        transform.position = managerC.enemyPool.transform.position;
        gameObject.SetActive(false);
    }
}
