using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public float MoveSpeed = 1;
    public NavMeshAgent Agent;
    public bool Leader = false;
    public Vector3 MoveEller = new(5, 5, 5);
    public EnemyStatus enemyStatus;
    public Animator Animator;
    [HideInInspector] public bool survive = true;
    [HideInInspector] public GameObject Player;
    [SerializeField] EnemyStatus _enemyStatus;
    [HideInInspector] public float timer = 3;
    public virtual void Move(Vector3 position)
    {
        if (survive)
        {
            Agent.speed = MoveSpeed;
            if (enemyStatus.Leader)
            {
                Agent.SetDestination(position);
            }
            else
            {
                Agent.SetDestination(position + new Vector3(Random.Range(MoveEller.x * -1, MoveEller.x), 0, Random.Range(MoveEller.z * -1, MoveEller.z)));
            }
        }
    }
    public virtual void Stop()
    {
        if (survive)
        {
            Agent.SetDestination(transform.position);
        }
    }

    void LeaderMove(ColliderHit hit)
    {
        if (hit.collider != null && !hit.collider.gameObject.CompareTag("PlayerHead"))
        {
            if (Agent.velocity.magnitude < 0.05f)
            {
                timer -= Time.deltaTime;
            }
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) > 300)
        {
            if (Agent.velocity.magnitude < 0.05f)
            {
                timer -= Time.deltaTime * 2;
            }
        }
        else
        {
            if (!Agent.isStopped)
                Invoke(nameof(Stop), Random.Range(1, 5));
            timer = 3f;
        }

        if (timer <= 0)
        {
            timer = 3;

            //プレイヤーを中心とした極座標に変換
            //極座標系で距離を近づけつつ周りをまわるように移動して射線を通すようにする。
            Vector3 local = Player.transform.position - transform.position;
            float dis = new Vector3(local.x, 0, local.z).magnitude;
            float theta = Mathf.Atan2(local.z, local.x);
            float checkTheta = theta + 0.8727f;
            float theta2;
            if (dis > 80)
                dis *= Random.Range(0.8f, 0.9f);
            else
                dis *= Random.Range(2.0f, 2.8f);
            //右側優先探査か左側優先探査かを調べる。これは右移動と左移動のどちらでも見えなかった場合は後に探査した方に移動することを示す。
            theta2 = _enemyStatus.LR == LR.left ? theta + ScalingRotate(dis) : theta - ScalingRotate(dis);
            Vector3 pointer = new(dis * Mathf.Cos(theta2), 0f, dis * Mathf.Sin(theta2));
            if (Physics.Raycast(pointer, (Player.transform.position - pointer).normalized, out RaycastHit hit2) && hit2.collider.gameObject.CompareTag("Player"))
            {
                theta2 = _enemyStatus.LR == LR.left ? theta - ScalingRotate(dis) : theta + ScalingRotate(dis);
            }
            else
            {
                theta2 = _enemyStatus.LR == LR.left ? theta + ScalingRotate(dis) : theta - ScalingRotate(dis);
            }
            pointer = Player.transform.TransformPoint(new Vector3(dis * Mathf.Cos(theta2), 0f, dis * Mathf.Sin(theta2)));
            Move(new(pointer.x, 0.5f, pointer.z));
            foreach (GameObject m in _enemyStatus.MembersList)
            {
                m.GetComponent<EnemyBase>().Move(new(pointer.x, 0.5f, pointer.z));
            }
        }
    }

    /// <summary>
    /// 移動の際に極座標で使うスケーリングした角度を返すメソッド
    /// 距離が遠ければ遠いほど角度が小さくなり最終的に20度になる
    /// </summary>
    /// <param name="dis">距離に応じて変えるので距離を入力</param>
    /// <returns></returns>
    float ScalingRotate(float dis)
    {
        /*
        if (dis < 20)
            return Mathf.PI / 3;//60度
        else if (dis > 40)
            return Mathf.PI / 9;//20度
        else
            return (-1.6f * dis + 92) * Mathf.PI / 180; //60度から20度の間で変化する二次式を弧度法に変換
        */
        if (dis < 20)
            return Mathf.PI;
        else if (dis > 60)
            return Mathf.PI / 6;
        else
            return (-3.75f * dis + 255) * Mathf.PI / 180;// 60度から20度の間で変化する二次式を弧度法に変換
    }

    public abstract void UniqueAction();
}