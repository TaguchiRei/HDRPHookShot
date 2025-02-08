using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] GameObject BlastObj;
    bool _hit = false;
    bool enemyHit = false;
    public float _speed = 50;
    [HideInInspector] public Vector3 _moveDirection = Vector3.zero;
    [HideInInspector] public Vector3 _hitPosition = Vector3.zero;
    [HideInInspector] public PlayerMove _playerMove;
    GameManager gameManager;
    GameObject player;
    [SerializeField] GameObject[] enemy;
    private void Start()
    {
        _moveDirection += transform.forward * _speed;
        _meshRenderer.enabled = false;
        gameManager = FindAnyObjectByType<GameManager>();
        player = FindAnyObjectByType<GameObject>();
    }

    private void Update()
    {
        if (!_hit && !gameManager._pause)
        {
            if (!enemyHit)
            {
                _rigidbody.linearVelocity = _moveDirection;
                _moveDirection *= 1.01f;
            }
            else
            {
                transform.LookAt(player.transform.position);
                _rigidbody.linearVelocity = (_playerMove.transform.position - transform.position).normalized * _speed / 2;
                if (Vector3.Distance(transform.position, player.transform.position) < 10)
                {
                    _playerMove.anchorTimer = 0;
                }
            }
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyStatus>().HookShotHit();
            enemyHit = true;
            var type = collision.gameObject.GetComponent<EnemyStatus>().EnemyType;
            switch (type)
            {
                case EnemyType.attacker:
                    enemy[0].SetActive(true);
                    break;
                case EnemyType.defender:
                    enemy[1].SetActive(true);
                    break;
                case EnemyType.supporter:
                    enemy[2].SetActive(true);
                    break;
                default:
                    break;
            }
        }
        else if (!enemyHit)
        {
            FindAnyObjectByType<PlayerMove>().HookShotHit = true;
            _hit = true;
            _rigidbody.linearVelocity = Vector3.zero;
            _meshRenderer.enabled = true;
        }
    }

    private void OnDestroy()
    {
        if (enemyHit)
        {
            Instantiate(BlastObj, transform.position, Quaternion.identity);
        }
    }
}
