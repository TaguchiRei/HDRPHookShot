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
        if (!hit.collider.gameObject.CompareTag("Player"))
        {
            //プレイヤーを中心とした極座標に変換
            Vector3 local = _player.transform.position - transform.position;
            float dis = Mathf.Sqrt(local.x * local.x + local.z * local.z);
            float theta = Mathf.Atan2(local.z, local.x);

            Debug.Log($"{dis} {theta + Mathf.PI}");
            //Move();
        }
    }
    public override void UniqueAction()
    {
        throw new System.NotImplementedException();
    }
}
