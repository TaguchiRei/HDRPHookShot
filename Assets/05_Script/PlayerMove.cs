using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    GameObject _gameManager;
    public bool _canMove = false;
    [SerializeField] float _moveSpeed = 1;
    public bool _canJump = false;
    [SerializeField] float _jumpPower = 1;

    [SerializeField] Rigidbody _rigidbody;
    Vector3 _movePower = Vector3.zero;
    bool _addForce = false;
    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    void Update()
    {
        //ÉvÉåÉCÉÑÅ[ÇÃìÆÇ´ÇçÏÇÈ
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ||Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            _movePower = transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"))) * _moveSpeed;
            _addForce = true;
        }
    }
    private void FixedUpdate()
    {
        if (_addForce)
        {
            _rigidbody.AddForce(_movePower);
            _movePower = Vector3.zero;
        }
    }
}
