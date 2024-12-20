using UnityEngine;

public class AttackerEnemyController : EnemyManager
{
    [HideInInspector] public GameObject _player;
    [SerializeField] GameObject _eye;

    float timer = 0;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        Physics.Raycast(transform.position, (_player.transform.position - transform.position).normalized, out RaycastHit hit);
        if (!hit.collider.gameObject.CompareTag("Player"))
        {
            if (_agent.velocity.magnitude < 0.05f)
            {
                timer -= Time.deltaTime;
            }
        }
        else
        {
            if (!_agent.isStopped)
                Invoke(nameof(Stop), Random.Range(4, 7));
            timer = 3f;
        }
        if (timer <= 0)
        {
            timer = 3;
            //プレイヤーを中心とした極座標に変換
            //極座標系で距離を近づけつつ周りをまわるように移動して射線を通すようにする。
            Vector3 local = _player.transform.position - transform.position;
            float dis = new Vector3(local.x, 0, local.z).magnitude;
            float theta = Mathf.Atan2(local.z, local.x);
            float checkTheta = theta + 0.8727f;
            float theta2 = theta + ScalingRotate(dis);
            dis *= 0.8f;
            Vector3 pointer = new Vector3(dis * Mathf.Cos(theta2), transform.position.y, dis * Mathf.Sin(theta2));
            if (Physics.Raycast(pointer, (_player.transform.position - pointer).normalized, out RaycastHit hit2) && hit2.collider.gameObject.CompareTag("Player"))
            {
                theta2 = theta - ScalingRotate(dis);
            }
            else
            {
                theta2 = theta + ScalingRotate(dis);
            }
            pointer = _player.transform.TransformPoint(new Vector3(dis * Mathf.Cos(theta2), 0f, dis * Mathf.Sin(theta2)));
            Move(new(pointer.x + Random.Range(-5, 5), 0.5f, pointer.z + Random.Range(-5, 5)));
        }

    }
    public override void UniqueAction()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 移動の際に極座標で使うスケーリングした角度を返すメソッド
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
            return (3.75f * dis + 255) * Mathf.PI / 180;
    }

    void Stop()
    {
        _agent.SetDestination(transform.position);
    }
}
