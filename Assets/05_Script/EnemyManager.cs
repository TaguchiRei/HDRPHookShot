using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyManager : MonoBehaviour
{
    [SerializeField] float _hp = 0;
    [SerializeField] float _moveSpeed = 0;
    [SerializeField] NavMeshAgent _agent;
    public virtual void Move(Vector3 position)
    {
        _agent.SetDestination(position);
    }

    public abstract void UniqueAction();

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