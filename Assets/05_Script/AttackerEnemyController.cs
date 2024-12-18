using UnityEngine;

public class AttackerEnemyController : EnemyManager
{
    [HideInInspector] public GameObject _player;
    [SerializeField] GameObject _eye;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        Physics.Raycast(transform.position, (_player.transform.position - transform.position).normalized, out RaycastHit hit);
        if (!hit.collider.gameObject.CompareTag("Player") && _agent.velocity.magnitude < 0.1f)
        {
            //プレイヤーを中心とした極座標に変換
            //極座標系で距離を近づけつつ周りをまわるように移動して射線を通すようにする。
            Vector3 local = _player.transform.position - transform.position;
            float dis = Mathf.Sqrt(local.x * local.x + local.z * local.z);
            float theta = Mathf.Atan2(local.z, local.x);
            float theta2 = theta + ScalingRotate(dis);
            Vector3 pointer = new Vector3(dis * Mathf.Cos(theta2), transform.position.y, dis * Mathf.Sin(theta2));
            Physics.Raycast(pointer, _player.transform.position - pointer,out RaycastHit hit2);
            if (!hit2.collider.gameObject.CompareTag("Player"))
            {
                theta2 = theta - ScalingRotate(dis);
                pointer = new Vector3(dis * Mathf.Cos(theta2), transform.position.y, dis * Mathf.Sin(theta2));
            }
            Move(pointer);
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
        if (dis < 20)
            return Mathf.PI;//180度
        else if (dis > 40)
            return Mathf.PI / 6;//30度
        else
            return (-6 * dis + 300) * Mathf.PI / 180; //180度から30度の間で変化する二次式を弧度法に変換
    }
}
