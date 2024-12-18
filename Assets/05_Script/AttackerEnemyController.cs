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
            //�v���C���[�𒆐S�Ƃ����ɍ��W�ɕϊ�
            //�ɍ��W�n�ŋ������߂Â�������܂��悤�Ɉړ����Ďː���ʂ��悤�ɂ���B
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
