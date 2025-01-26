using TMPro.EditorUtilities;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Rigidbody _rigidbody;
    bool _hit = false;
    public float _speed = 50;
    [HideInInspector] public Vector3 _moveDirection = Vector3.zero;
    [HideInInspector] public Vector3 _hitPosition = Vector3.zero;
    GameManager gameManager;
    private void Start()
    {
        _moveDirection += transform.forward * _speed;
        _meshRenderer.enabled = false;
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (!_hit && !gameManager._pause)
        {
            _rigidbody.linearVelocity = _moveDirection;
            _moveDirection *= 1.01f;
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        FindAnyObjectByType<PlayerMove>().HookShotHit = true;
        _hit = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _meshRenderer.enabled = true;
    }
}
