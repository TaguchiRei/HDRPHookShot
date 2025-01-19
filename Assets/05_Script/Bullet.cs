using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameObject _player;
    GameObject _playerEye;
    public bool Homing = false;
    public bool Railgun = false;
    public float Dmg = 0;
    float _bulletSpeed = 1;
    float _railgunAttackRange = 1;
    [SerializeField] Rigidbody _rig;
    private void Start()
    {
        transform.position = _playerEye.transform.position;
        transform.SetPositionAndRotation(_playerEye.transform.position, _playerEye.transform.rotation);
    }

    private void FixedUpdate()
    {
        _rig.linearVelocity = transform.forward * _bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

        }
    }
}
