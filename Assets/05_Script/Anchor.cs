using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] Rigidbody _rigidbody;
    bool _hit = false;
    public float _speed = 50;
    [HideInInspector] public Vector3 _moveDirection = Vector3.zero;
    [HideInInspector] public Vector3 _hitPosition = Vector3.zero;
    private void Start()
    {
        _moveDirection += transform.forward * _speed;
        _meshRenderer.enabled = false;
    }

    private void Update()
    {
        if (!_hit)
        {
            _rigidbody.linearVelocity = _moveDirection;
        }
        else
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        FindAnyObjectByType<PlayerMove>()._hookShotHit = true;
        _hit = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _meshRenderer.enabled = true;
    }
}
