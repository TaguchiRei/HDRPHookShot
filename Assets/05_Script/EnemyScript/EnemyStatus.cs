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
    private GameObject managerObj;
    private int Hp = 3;

    public void Initialization(int groupNumber, LR lr, bool isLeader, GameObject manager, bool summon = true)
    {
        Hp = 3;
        GroupNumber = groupNumber;
        LR = lr;
        Leader = isLeader;
        managerObj = manager;
        enemyBase.Animator.SetBool("Delete", false);
        if (summon)
        {
            agent.enabled = true;
            enemyBase.enabled = true;
            enemyBase.survive = true;
        }
    }

    public void HPChanger(int changeNum)
    {
        Hp -= changeNum;
        Debug.Log(changeNum + " " + Hp);
        if (Hp <= 0)
        {
            //Ž€–SŽžˆ—
            enemyBase.survive = false;
            agent.enabled = false;
            enemyBase.enabled = false;
            enemyBase.Animator.SetBool("Delete", true);
        }
        else if (Hp > _maxHp)
        {
            Hp = _maxHp;
        }
    }
    public void HookShotHit()
    {

    }

    void Deleted()
    {
        var managerC = managerObj.GetComponent<EnemyManager>();
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
