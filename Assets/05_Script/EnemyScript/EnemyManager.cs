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
    [SerializeField] List<Vector3> spawnPoint = new();
    [SerializeField] float spawnRange = 0;
    [Tooltip("�O���[�v�����ꂼ�ꂢ�����邩������")]
    [SerializeField] GroupNumber group;
    [Tooltip("�O���[�v�ɂ��ꂼ�ꉽ�l���邩������")]
    [SerializeField] GroupNumber groupNum;

    
    public Queue<GameObject> AttackerQueue = new Queue<GameObject>();
    public Queue<GameObject> SupporterQueue = new Queue<GameObject>();
    public Queue<GameObject> DefenderQueue = new Queue<GameObject>();
    public int WaitingForSpawn = 0;


    private void Start()
    {
        StartCoroutine(ButtleStart(0));
    }
    IEnumerator ButtleStart(int Stage)
    {
        //���g�̃}�b�v�ԍ��̃G�l�~�[�f�[�^���擾
        foreach (EnemyDataFormat e in stageEnemyData.enemyAndNumbers[Stage].enemy)
        {
            for (int i = 0; i < e.number; i++)
            {
                allEnemyData.Add(e.enemy);
            }
        }
        //�擾�����I�u�W�F�N�g���V���b�t������
        allEnemyData = allEnemyData.OrderBy(d => Guid.NewGuid()).ToList();//�����_���Ƀ\�[�g���邱�Ƃŏo�Ă��鏇�Ԃ��΂炷

        int groupMemberNum = groupNum.Big;
        for (int i = 0; i < group.Small + group.Middle + group.Big; i++)
        {
            if (group.Big + group.Middle < i + 1)
                groupMemberNum = groupNum.Small;
            else if (group.Big < i + 1)
                groupMemberNum = groupNum.Middle;


            List<GameObject> enemyList = allEnemyData.GetRange(0, groupMemberNum);
            allEnemyData.RemoveRange(0, groupMemberNum);
            //���[�_�[�ƂȂ�I�u�W�F�N�g�𐶐��Ə�����
            Vector3 spawnerPos = spawnPoint[UnityEngine.Random.Range(0, spawnPoint.Count)];//����������W�������_������
            GameObject leaderObj = Instantiate(enemyList[0], spawnerPos, Quaternion.identity);
            leaderObj.name = Guid.NewGuid().ToString();
            EnemyStatus leaderSta = leaderObj.GetComponent<EnemyStatus>();
            LR lR = (LR)UnityEngine.Random.Range(0, 2);
            leaderSta.Initialization(i, lR, true, gameObject,leaderObj);
            enemyList.RemoveAt(0);


            HashSet<GameObject> enemyHashSet = enemyList.ToHashSet();
            List<GameObject> resultGroup = new();//���̉�Ő��������O���[�v��ۑ�����
            
            //����^�C�v�̃G�l�~�[���ƂɈ�C�ɐ���
            foreach (GameObject enemy in enemyHashSet)
            {
                var instantiateResult = InstantiateAsync(
                    enemy,
                    enemyList.Count(obj => obj == enemy),
                    leaderObj.transform.position,
                    Quaternion.identity);
                yield return instantiateResult;
                resultGroup.AddRange(instantiateResult.Result);
            }
            Debug.Log("enemyGenerated");
            foreach (var obj in resultGroup)
            {
                obj.name = Guid.NewGuid().ToString();
                obj.transform.position = obj.transform.position + new Vector3(UnityEngine.Random.Range(-1 * spawnRange, spawnRange + 1), 0, UnityEngine.Random.Range(-1 * spawnRange, spawnRange + 1));//�ʒu������
                EnemyStatus enemyStatus = obj.GetComponent<EnemyStatus>();
                enemyStatus.Initialization(i, lR, false, gameObject, leaderObj);
            }
            Debug.Log("enemyInitialize");
            leaderSta.MembersList = resultGroup;
        }
        if(allEnemyData.Count != 0)
        {
            HashSet<GameObject> enemyHashSet = allEnemyData.ToHashSet();
            foreach (var obj in enemyHashSet)
            {
                
            }
        }
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var v in spawnPoint)
        {
            Gizmos.DrawWireSphere(v, spawnRange);
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