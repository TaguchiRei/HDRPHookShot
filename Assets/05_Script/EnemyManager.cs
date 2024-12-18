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
    /// �ړ��̍ۂɋɍ��W�Ŏg���X�P�[�����O�����p�x��Ԃ����\�b�h
    /// </summary>
    /// <param name="dis">�����ɉ����ĕς���̂ŋ��������</param>
    /// <returns></returns>
    float ScalingRotate(float dis)
    {
        if (dis < 20)
            return Mathf.PI;//180�x
        else if (dis > 40)
            return Mathf.PI / 6;//30�x
        else
            return (-6 * dis + 300) * Mathf.PI / 180; //180�x����30�x�̊Ԃŕω�����񎟎����ʓx�@�ɕϊ�
    }
}