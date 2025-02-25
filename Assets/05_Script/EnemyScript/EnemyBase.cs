using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public float MoveSpeed = 1;
    public NavMeshAgent Agent;
    public bool Leader = false;
    public Vector3 MoveEller = new(5, 5, 5);
    public EnemyStatus EnemyStatus;
    public Animator Animator;
    [HideInInspector] public bool Survive = true;
    [HideInInspector] public GameObject PlayerHead;
    [HideInInspector] public float Timer = 3;
    float actionInterval;
    [HideInInspector] public Vector3 DelayedPosition = Vector3.zero;
    Vector3 direction;
    [SerializeField] float _delay = 1f;
    private Queue<Vector3> positionHistory = new();
    float elapsedTime = 1;
    bool DelayedUniqueAction = false;
    Coroutine movingCoroutine;
    public PlayerMove PlayerMoveI;
    public bool CanMove = false;

    public EnemyManager EnemyManager;

    public virtual void Start()
    {
        PlayerMoveI = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        PlayerHead = GameObject.FindGameObjectWithTag("PlayerHead");
    }
    public virtual void Update()
    {
        if (Survive && !EnemyManager.Stop)
        {
            Agent.isStopped = false;
            Animator.speed = 1f;
            if (Agent.remainingDistance <= 0.5f && !Agent.hasPath)
            {
                Animator.SetBool("Walking", false);
            }
            else
            {
                Animator.SetBool("Walking", true);
            }
            if (EnemyStatus.Leader)
            {
                LeaderMove();
                positionHistory.Enqueue(PlayerHead.transform.position);
                elapsedTime += Time.deltaTime;
                actionInterval -= Time.deltaTime;
                if (elapsedTime >= _delay)
                {
                    if (positionHistory.Count != 0)
                    {
                        DelayedPosition = positionHistory.Dequeue();
                        if (actionInterval < 0 && !DelayedUniqueAction && DelayedPosition != null)
                        {
                            actionInterval = Random.Range(2.0f, 3.0f);
                            DelayedUniqueAction = true;
                            UniqueAction(DelayedPosition);
                            movingCoroutine = StartCoroutine(DelayUniqueAction(DelayedPosition));
                        }
                    }
                }
            }
        }
        else
        {
            Animator.speed = 0;
            Agent.isStopped = true;
            Agent.velocity = Vector3.zero;
        }
    }
    public virtual void Move(Vector3 position)
    {
        if (Survive && CanMove)
        {
            Agent.speed = MoveSpeed;
            if (EnemyStatus.Leader)
            {
                Agent.SetDestination(position);
            }
            else
            {
                Agent.SetDestination(position + new Vector3(Random.Range(MoveEller.x * -1, MoveEller.x), 0, Random.Range(MoveEller.z * -1, MoveEller.z)));
            }
        }
    }
    public virtual void Delete()
    {
        if (DelayedUniqueAction)
        {
            DelayedUniqueAction = false;
            StopCoroutine(movingCoroutine);
        }
    }
    public virtual void Stop()
    {
        if (Survive)
        {
            Agent.SetDestination(transform.position);
        }
    }

    public void LeaderMove()
    {
        Physics.Raycast(transform.position, (PlayerHead.transform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground", "PlayerHead"));
        if (hit.collider != null && !hit.collider.gameObject.CompareTag("PlayerHead"))
        {
            if (Agent.velocity.magnitude < 0.05f)
            {
                Timer -= Time.deltaTime;
            }
        }
        else if (Vector3.Distance(transform.position, PlayerHead.transform.position) > 300)
        {
            if (Agent.velocity.magnitude < 0.05f)
            {
                Timer -= Time.deltaTime * 2;
            }
        }
        else
        {
            if (!Agent.isStopped)
                Invoke(nameof(Stop), Random.Range(1, 5));
            Timer = 3f;
        }

        if (Timer <= 0)
        {
            Timer = 3;

            Vector3 pointer;
            Vector3 local = PlayerHead.transform.position - transform.position;
            float dis = new Vector3(local.x, 0, local.z).magnitude;
            float theta = Mathf.Atan2(local.z, local.x);
            float checkTheta = theta + 0.8727f;
            float theta2;
            if (Vector3.Distance(transform.position, PlayerHead.transform.position) > 300)
            {
                //プレイヤーを中心とした極座標に変換
                //極座標系で距離を近づけつつ周りをまわるように移動して射線を通すようにする。
                if (dis > 80)
                    dis *= Random.Range(0.8f, 0.9f);
                else
                    dis *= Random.Range(2.0f, 2.8f);
                //右側優先探査か左側優先探査かを調べる。これは右移動と左移動のどちらでも見えなかった場合は後に探査した方に移動することを示す。
                theta2 = EnemyStatus.LR == LR.left ? theta + ScalingRotate(dis) : theta - ScalingRotate(dis);
                pointer = new(dis * Mathf.Cos(theta2), 0f, dis * Mathf.Sin(theta2));
                if (Physics.Raycast(pointer, (PlayerHead.transform.position - pointer).normalized, out RaycastHit hit2) && hit2.collider.gameObject.CompareTag("PlayerHead"))
                {
                    theta2 = EnemyStatus.LR == LR.left ? theta - ScalingRotate(dis) : theta + ScalingRotate(dis);
                }
                else
                {
                    theta2 = EnemyStatus.LR == LR.left ? theta + ScalingRotate(dis) : theta - ScalingRotate(dis);
                }
                pointer = PlayerHead.transform.TransformPoint(new Vector3(dis * Mathf.Cos(theta2), 0f, dis * Mathf.Sin(theta2)));
            }
            else
            {
                dis *= 0.5f;
                pointer = PlayerHead.transform.TransformPoint(new Vector3(dis * Mathf.Cos(theta), 0f, dis * Mathf.Sin(theta)));
            }
            //選択された範囲が外に出すぎているときはエリア内に丸める
            if (pointer.x < -500)
                pointer.x = -495f;
            else if (pointer.x > 500)
                pointer.x = 495f;

            if (pointer.z < -500)
                pointer.z = -495f;
            else if (pointer.z > 500)
                pointer.z = 495f;

            Move(new(pointer.x, 0.5f, pointer.z));
            foreach (EnemyBase eb in EnemyStatus.EnemyBaseList)
            {
                eb.Move(new(pointer.x, 0.5f, pointer.z));
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
        if (dis < 20)
            return Mathf.PI;
        else if (dis > 60)
            return Mathf.PI / 6;
        else
            return (-3.75f * dis + 255) * Mathf.PI / 180;// 60度から20度の間で変化する二次式を弧度法に変換
    }

    public abstract void UniqueInitialization();
    public abstract void UniqueAction(Vector3 delayedPosition);
    public IEnumerator DelayUniqueAction(Vector3 delayedPos)
    {
        for (int i = 0; i < EnemyStatus.EnemyBaseList.Count; i++)
        {
            yield return new WaitForEndOfFrame();
            if (i < EnemyStatus.EnemyBaseList.Count && EnemyStatus.EnemyBaseList[i].enabled)
                EnemyStatus.EnemyBaseList[i].UniqueAction(delayedPos);
        }
        DelayedUniqueAction = false;
    }
}