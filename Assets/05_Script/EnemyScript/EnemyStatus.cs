using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour,IEnemyInterface
{
    public int GroupNumber { get; set; }
    public LR LR { get; set; }
    public bool Leader { get; set; }

    [SerializeField] private EnemyStatus enemyStatus;

    [SerializeField] EnemyBase enemyBase;
    [SerializeField] int _maxHp;
    public List<GameObject> MembersList;
    GameObject managerObj;
    GameObject leaderObj;
    public int Hp = 3;
    
    public void Initialization(int groupNumber, LR lr, bool isLeader, GameObject manager, GameObject leader)
    {
        GroupNumber = groupNumber;
        LR = lr;
        Leader = isLeader;
        managerObj = manager;
        leaderObj = leader;
    }

    public void HPChanger(int changeNum)
    {
        Hp -= changeNum;
        if (Hp > 0)
        {
            //Ž€–SŽžˆ—

        }
        else if (Hp > _maxHp)
        {
            Hp = _maxHp;
        }
    }
    public void HookShotHit()
    {

    }
    public void InQueue()
    {

    }
}
