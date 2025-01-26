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
    [Tooltip("グループがそれぞれいくつあるかを入れる")]
    [SerializeField] GroupNumber group;
    [Tooltip("グループにそれぞれ何人入るかを入れる")]
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
        //自身のマップ番号のエネミーデータを取得
        foreach (EnemyDataFormat e in stageEnemyData.enemyAndNumbers[Stage].enemy)
        {
            for (int i = 0; i < e.number; i++)
            {
                allEnemyData.Add(e.enemy);
            }
        }
        //取得したオブジェクトをシャッフルする
        allEnemyData = allEnemyData.OrderBy(d => Guid.NewGuid()).ToList();//ランダムにソートすることで出てくる順番をばらす

        int groupMemberNum = groupNum.Big;
        for (int i = 0; i < group.Small + group.Middle + group.Big; i++)
        {
            if (group.Big + group.Middle < i + 1)
                groupMemberNum = groupNum.Small;
            else if (group.Big < i + 1)
                groupMemberNum = groupNum.Middle;


            List<GameObject> enemyList = allEnemyData.GetRange(0, groupMemberNum);
            allEnemyData.RemoveRange(0, groupMemberNum);
            //リーダーとなるオブジェクトを生成と初期化
            Vector3 spawnerPos = spawnPoint[UnityEngine.Random.Range(0, spawnPoint.Count)];//生成する座標をランダム決定
            GameObject leaderObj = Instantiate(enemyList[0], spawnerPos, Quaternion.identity);
            leaderObj.name = Guid.NewGuid().ToString();
            EnemyStatus leaderSta = leaderObj.GetComponent<EnemyStatus>();
            LR lR = (LR)UnityEngine.Random.Range(0, 2);
            leaderSta.Initialization(i, lR, true, gameObject,leaderObj);
            enemyList.RemoveAt(0);


            HashSet<GameObject> enemyHashSet = enemyList.ToHashSet();
            List<GameObject> resultGroup = new();//この回で生成したグループを保存する
            
            //同一タイプのエネミーごとに一気に生成
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
                obj.transform.position = obj.transform.position + new Vector3(UnityEngine.Random.Range(-1 * spawnRange, spawnRange + 1), 0, UnityEngine.Random.Range(-1 * spawnRange, spawnRange + 1));//位置を決定
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