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
    bool suvive = true;

    public void Initialization(int groupNumber, LR lr, bool isLeader, GameObject manager,GameObject leaderObj, bool summon = true)
    {
        Hp = _maxHp;
        GroupNumber = groupNumber;
        LR = lr;
        Leader = isLeader;
        enemyBase.Leader = isLeader;
        managerObject = manager;
        leaderObject = leaderObj;
        suvive = true;
        enemyBase.Animator.SetBool("Delete", false);
        agent.enabled = true;
        enemyBase.enabled = true;
        enemyBase.survive = true;
        if (!Leader)
        {
            leaderObject.GetComponent<EnemyStatus>().MembersList.Add(gameObject);
            enemyBase.Move(leaderObject.transform.position);
        }
    }

    public void HPChanger(int changeNum)
    {
        if (suvive)
        {
            Hp -= changeNum;
            if (Hp <= 0)
            {
                suvive = false;
                leaderObject.GetComponent<EnemyStatus>().MembersList.Remove(gameObject);
                //éÄñSéûèàóù
                enemyBase.survive = false;
                agent.enabled = false;
                enemyBase.enabled = false;
                enemyBase.Animator.SetBool("Delete", true);
                managerObject.GetComponent<EnemyManager>().EnemySpawn(GroupNumber, LR, Leader,leaderObject, MembersList);
            }
            else if (Hp > _maxHp)
            {
                Hp = _maxHp;
            }
        }
    }
    public void HookShotHit()
    {

    }

    void Deleted()
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
