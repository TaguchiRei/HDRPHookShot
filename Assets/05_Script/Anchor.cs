using UnityEngine;

public class Anchor : MonoBehaviour
{
    [SerializeField] GameObject _muzzle;
    [SerializeField] GameObject _camera;
    [SerializeField] GameObject _gun;
    [SerializeField] MeshRenderer _meshRenderer;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Rigidbody _rig;
    bool _shot = false;
    bool _returnAnc = false;
    float _timer = 0;
    int _speed = 0;

    private void Start()
    {
        _lineRenderer.enabled = false;
    }
    private void Update()
    {
        if (_shot)
        {
            _rig.linearVelocity = transform.forward * _speed;
        }
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer < 0)
            {
                _meshRenderer.enabled = true;
                _rig.linearVelocity = Vector3.zero;
            }
        }
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1,_muzzle.transform.position);
    }

    public void AnchorShot(float ancSpeed = 50)
    {
        Debug.Log("Anc");
        _speed = (int)ancSpeed;
        _returnAnc = false;
        transform.parent = null;
        _meshRenderer.enabled = false;
        transform.SetPositionAndRotation(_camera.transform.position, _camera.transform.rotation);
        _rig.linearVelocity = transform.forward *  _speed;
        _lineRenderer.enabled = true;
    }
    public void AnchorReset()
    {
        _returnAnc = true;
        _meshRenderer.enabled = false;
        transform.SetParent(_gun.transform);
        transform.localPosition = new Vector3(-0.5f, -5f, 0);
        transform.localEulerAngles = new Vector3(-0.55f,-92.5f,-1.4f);
        _timer = 1f;
        _lineRenderer.enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {

        }
        else
        {
            _shot = false;
            _rig.linearVelocity = Vector3.zero;
            _meshRenderer.enabled=true;
        }
    }
}
