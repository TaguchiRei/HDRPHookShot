using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour,IEnemyInterface
{
    public int GroupNumber { get; set; }
    public LR LR { get; set; }
    public bool Leader { get; set; }

    public GameObject LeaderObject;
    [SerializeField] EnemyBase enemyBase;

    public List<GameObject> MembersList;
    public void Initialization(int groupNumber, LR lr, bool leader, GameObject leaderObj)
    {
        GroupNumber = groupNumber;
        LR = lr;
        Leader = leader;
        LeaderObject = leaderObj;
    }
}
