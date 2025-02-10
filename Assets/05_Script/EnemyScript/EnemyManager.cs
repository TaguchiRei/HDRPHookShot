using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<EnemyGroup> enemyGroups = new();
    List<GameObject> allEnemyData = new();
    [SerializeField] bool infiniteEnemySpawn = false;
    [SerializeField] EnemyData stageEnemyData;
    [SerializeField] Vector3[] spawnPoint;
    [SerializeField] float spawnRange = 0;
    [Tooltip("グループがそれぞれいくつあるかを入れる")]
    [SerializeField] GroupNumber group;
    [Tooltip("グループにそれぞれ何人入るかを入れる")]
    [SerializeField] GroupNumber groupNum;

    public GameObject enemyPool;
    public Queue<GameObject> AttackerQueue = new();
    public Queue<GameObject> SupporterQueue = new();
    public Queue<GameObject> DefenderQueue = new();
    public int WaitingForSpawn = 0;
    public bool _measurement = false;
    public int _measurementNum = 0;

    public IEnumerator ButtleStart(int Stage)
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
        List<AttackerEnemyController> leader = new();
        for (int i = 0; i < group.Small + group.Middle + group.Big; i++)
        {
            if (group.Big + group.Middle < i + 1)
                groupMemberNum = groupNum.Small;
            else if (group.Big < i + 1)
                groupMemberNum = groupNum.Middle;
            List<GameObject> enemyList = allEnemyData.GetRange(0, groupMemberNum);
            allEnemyData.RemoveRange(0, groupMemberNum);
            //リーダーとなるオブジェクトを生成と初期化
            Vector3 spawnerPos = spawnPoint[UnityEngine.Random.Range(0, spawnPoint.Length)];//生成する座標をランダム決定
            GameObject leaderObj = Instantiate(enemyList[0], spawnerPos, Quaternion.identity);
            EnemyStatus leaderSta = leaderObj.GetComponent<EnemyStatus>();
            LR lR = (LR)UnityEngine.Random.Range(0, 2);
            leaderSta.Initialization(i, lR, true, gameObject, leaderObj);
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
            foreach (var obj in resultGroup)
            {
                obj.transform.position = obj.transform.position + new Vector3(UnityEngine.Random.Range(-1 * spawnRange, spawnRange + 1), 0, UnityEngine.Random.Range(-1 * spawnRange, spawnRange + 1));//位置を決定
                EnemyStatus enemyStatus = obj.GetComponent<EnemyStatus>();
                enemyStatus.Initialization(i, lR, false, gameObject, leaderObj);
            }
            leaderSta.MembersList = resultGroup;
        }
        if (allEnemyData.Count != 0)
        {
            HashSet<GameObject> enemyHashSet = allEnemyData.ToHashSet();
            foreach (var obj in enemyHashSet)
            {
                var instantiateResult = InstantiateAsync(
                    obj,
                    allEnemyData.Count(enemyType => enemyType == obj),
                    enemyPool.transform.position,
                    Quaternion.identity
                    );
                yield return instantiateResult;
                var objType = obj.GetComponent<EnemyStatus>().EnemyType;
                if (objType == EnemyType.attacker)
                {
                    foreach (var r in instantiateResult.Result)
                    {
                        r.SetActive(false);
                        AttackerQueue.Enqueue(r);
                    }
                }
                else if (objType == EnemyType.defender)
                {
                    foreach (var r in instantiateResult.Result)
                    {
                        r.SetActive(false);
                        SupporterQueue.Enqueue(r);
                    }
                }
                else
                {
                    foreach (var r in instantiateResult.Result)
                    {
                        r.SetActive(false);
                        DefenderQueue.Enqueue(r);
                    }
                }
            }
        }
    }

    public void EnemySpawn(int teamNumber, LR lR, bool leader, GameObject leaderObj, List<GameObject> memberList = null)
    {
        if (_measurement)
            _measurementNum++;
        GameObject spawnObj;
        if (infiniteEnemySpawn)
        {
            //スポーンさせる敵をランダムで決定。
            var spawnType = UnityEngine.Random.Range(0, AttackerQueue.Count + DefenderQueue.Count + SupporterQueue.Count);
            if (spawnType < AttackerQueue.Count)
            {
                spawnObj = AttackerQueue.Dequeue();
            }
            else if (spawnType < AttackerQueue.Count + DefenderQueue.Count)
            {
                spawnObj = DefenderQueue.Dequeue();
            }
            else if (SupporterQueue.Count != 0)
            {
                spawnObj = SupporterQueue.Dequeue();
            }
            else
            {
                return;
            }
        }
        else if (allEnemyData.Count != 0)
        {
            var type = allEnemyData[0].GetComponent<EnemyStatus>().EnemyType;
            //スポーンさせる敵を最初に決めたランダムなスポーン順で決定
            if (type == EnemyType.attacker && AttackerQueue.Count ==0 || type == EnemyType.defender && DefenderQueue.Count == 0 || type == EnemyType.supporter && SupporterQueue.Count == 0)
            {
                return;
            }
            
            spawnObj = type switch
            {
                EnemyType.attacker => AttackerQueue.Dequeue(),
                EnemyType.defender => DefenderQueue.Dequeue(),
                EnemyType.supporter => SupporterQueue.Dequeue(),
                _ => null,
            };
            allEnemyData.RemoveAt(0);
        }
        else
        {
            return;
        }
        spawnObj.SetActive(true);
        if (spawnObj != null)
        {
            //スポーンさせる敵の初期化を行う
            var spawnStatus = spawnObj.GetComponent<EnemyStatus>();
            spawnObj.transform.position = spawnPoint[UnityEngine.Random.Range(0, spawnPoint.Length)];
            spawnStatus.MembersList = memberList;
            spawnStatus.Initialization(
                teamNumber,
                lR,
                leader,
                gameObject,
                leaderObj
                );
            spawnObj.GetComponent<EnemyBase>().UniqueInitialization();
        }
        else
        {
            return;
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