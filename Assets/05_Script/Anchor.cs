using TMPro.EditorUtilities;
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
                _rigidbody.linearVelocity = (player.transform.position - transform.position).normalized * _speed;
                if(Vector3.Distance(transform.position, player.transform.position) < 10)
                {

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
            //FindAnyObjectByType<PlayerMove>().
        }
        else
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
