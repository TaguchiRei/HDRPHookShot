using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour,IEnemyInterface
{
    public int GroupNumber { get; set; }
    public LR LR { get; set; }
    public bool Leader { get; set; }

    public GameObject LeaderObject;
    [SerializeField] EnemyBace enemyBace;

    public List<EnemyStatus> MemberStatusList;
    public void Initialization(int groupNumber, LR lr, bool leader, GameObject leaderObj)
    {
        GroupNumber = groupNumber;
        LR = lr;
        Leader = leader;
        LeaderObject = leaderObj;
    }
}
