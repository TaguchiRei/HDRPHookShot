using GamesKeystoneFramework.PolarCoordinates;
using UnityEngine;

public class AttackerEnemyController : EnemyBace
{
    [HideInInspector] public GameObject Player;
    [SerializeField] GameObject _eye;
    [SerializeField] EnemyStatus _enemyStatus;
    [SerializeField] Animator _animator;
    float timer = 0;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        Physics.Raycast(transform.position, (Player.transform.position - transform.position).normalized, out RaycastHit hit);
        if (Agent.remainingDistance <= 0.5f && !Agent.hasPath)
        {
            _animator.SetBool("Walking", false);
        }
        else
        {
            _animator.SetBool("Walking", true);
        }
        if (hit.collider != null && !hit.collider.gameObject.CompareTag("Player"))
        {
            if (Agent.velocity.magnitude < 0.05f)
            {
                timer -= Time.deltaTime;
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
            
            if(Vector3.Distance(transform.position,Player.transform.position) < 80)
            {
                //�v���C���[�𒆐S�Ƃ����ɍ��W�ɕϊ�
                //�ɍ��W�n�ŋ������߂Â�������܂��悤�Ɉړ����Ďː���ʂ��悤�ɂ���B
                Vector3 local = Player.transform.position - transform.position;
                float dis = new Vector3(local.x, 0, local.z).magnitude;
                float theta = Mathf.Atan2(local.z, local.x);
                float checkTheta = theta + 0.8727f;
                float theta2;
                dis *= 0.8f;
                //�E���D��T���������D��T�����𒲂ׂ�B����͉E�ړ��ƍ��ړ��̂ǂ���ł������Ȃ������ꍇ�͌�ɒT���������Ɉړ����邱�Ƃ������B
                theta2 = _enemyStatus.LR == LR.left ? theta + ScalingRotate(dis) : theta - ScalingRotate(dis);
                Vector3 pointer = new(dis * Mathf.Cos(theta2), transform.position.y, dis * Mathf.Sin(theta2));
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
            }
            else
            {
                Vector3 local = Player.transform.position - transform.position;
                PolarCoordinates p = PolarCoordinatesSupport.ToPolarCoordinates(local);
                p.radius *= 0.5f;
                Move(p.ToVector2());
            }
        }
    }
    public override void UniqueAction()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// �ړ��̍ۂɋɍ��W�Ŏg���X�P�[�����O�����p�x��Ԃ����\�b�h
    /// ������������Ή����قǊp�x���������Ȃ�ŏI�I��20�x�ɂȂ�
    /// </summary>
    /// <param name="dis">�����ɉ����ĕς���̂ŋ��������</param>
    /// <returns></returns>
    float ScalingRotate(float dis)
    {
        /*
        if (dis < 20)
            return Mathf.PI / 3;//60�x
        else if (dis > 40)
            return Mathf.PI / 9;//20�x
        else
            return (-1.6f * dis + 92) * Mathf.PI / 180; //60�x����20�x�̊Ԃŕω�����񎟎����ʓx�@�ɕϊ�
        */
        if (dis < 20)
            return Mathf.PI;
        else if (dis > 60)
            return Mathf.PI / 6;
        else
            return (-3.75f * dis + 255) * Mathf.PI / 180;// 60�x����20�x�̊Ԃŕω�����񎟎����ʓx�@�ɕϊ�
    }

    public override void Move(Vector3 position)
    {
        base.Move(position);
        _animator.SetBool("Walking", true);
    }
}
