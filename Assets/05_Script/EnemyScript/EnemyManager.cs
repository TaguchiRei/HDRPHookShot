using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<EnemyGroup> enemyGroups = new();
    List<GameObject> allEnemyData = new();
    [SerializeField] EnemyData stageEnemyData;
    [SerializeField] List<Vector3> spornPoint = new();
    [SerializeField] float spornRange = 0;
    [SerializeField] int groupNumber;
    [Tooltip("�O���[�v�����ꂼ�ꂢ�����邩������")]
    [SerializeField] GroupNumber group;
    [Tooltip("�O���[�v�ɂ��ꂼ�ꉽ�l���邩������")]
    [SerializeField] GroupNumber groupNum;

    public void ButtleStart(int Stage)
    {
        foreach (EnemyDataFormat e in stageEnemyData.enemyAndNumbers[Stage].enemy)
        {
            for (int i = 0; i < e.number; i++)
            {
                allEnemyData.Add(e.enemy);
            }
        }
        allEnemyData = allEnemyData.OrderBy(d => Guid.NewGuid()).ToList();//�����_���Ƀ\�[�g���邱�Ƃŏo�Ă��鏇�Ԃ��΂炷
        int groupSize = group.Small;
        for (int i = 0; i < group.Big + group.Middle + group.Small; i++)
        {
            if (i > group.Small + group.Middle)
                groupSize = group.Big;
            else if (i > group.Small)
                groupSize = group.Middle;
            enemyGroups.Add(new()
            {
                GroupNumber = i,
                MaxMember = groupSize,
                LR = (LR)UnityEngine.Random.Range(0, 2),
                MemberList = new()
            });
            enemyGroups[i].MemberList = allEnemyData.GetRange(0, groupNum.Big);
            allEnemyData.RemoveRange(0, groupNum.Big);
        }
    }
    IEnumerator GenerateEnemyGroups(List<EnemyGroup> generateEnemyGroups)
    {
        GameObject groupLeader;
        EnemyStatus LeaderStatus;
        HashSet<GameObject> enemyType;
        for (int i = 0; i < generateEnemyGroups.Count; i++)
        {
            //���[�_�[���쐬
            LR lR = (LR)UnityEngine.Random.Range(0, 2);
            groupLeader = Instantiate(generateEnemyGroups[i].MemberList[0], spornPoint[i], Quaternion.identity);
            generateEnemyGroups[i].MemberList.RemoveAt(0);
            LeaderStatus = groupLeader.GetComponent<EnemyStatus>();
            LeaderStatus.Initialization(i, lR, true, groupLeader);
            LeaderStatus.MemberStatusList = new();
            //�O���[�v���쐬
            enemyType = generateEnemyGroups[i].MemberList.ToHashSet();//�n�b�V���Z�b�g�ɂ��ă^�C�v�����擾
            foreach (var member in enemyType)
            {
                var instantiateOperation = InstantiateAsync(member, generateEnemyGroups[i].MemberList.Count(countMember => countMember = member));
                yield return instantiateOperation;
                var resultArr = instantiateOperation.Result.Where(obj => obj != null).Select(obj => obj.GetComponent<EnemyStatus>());
                foreach (var enemyInstance in instantiateOperation.Result)
                {
                    //�����ʒu������
                    enemyInstance.GetComponent<EnemyStatus>().Initialization(i, lR, false, groupLeader);
                    enemyInstance.transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up);
                    enemyInstance.transform.position += enemyInstance.transform.position + transform.forward * UnityEngine.Random.Range(0.0f, spornRange);
                }
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var v in spornPoint)
        {
            Gizmos.DrawWireSphere(v, spornRange);
        }
    }
#endif
}
public class EnemyGroup : MonoBehaviour
{
    public int GroupNumber;
    public int MaxMember;
    public LR LR;
    public List<GameObject> MemberList;
}
public enum LR
{
    left = 0,
    right = 1,
}

[Serializable]
public class GroupNumber
{
    public int Big;
    public int Middle;
    public int Small;
}