using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMove : MonoBehaviour
{
    GameObject _gameManager;
    public bool _canMove = false;
    [SerializeField] float _moveSpeed = 1;
    public bool _canJump = false;
    [SerializeField] float _jumpPower = 1;

    [SerializeField] float _gravityScale = 1;
    [SerializeField] float[] _gravityScaleChangePoint;
    [SerializeField] Rigidbody _rigidbody;
    Vector3 _movePower = Vector3.zero;
    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    void Update()
    {
        //プレイヤーの動きを作る
        _movePower = transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal") * _moveSpeed, Input.GetAxisRaw("Jump") * _jumpPower, Input.GetAxisRaw("Vertical") * _moveSpeed));
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S))
        {
            
        }
        _rigidbody.AddForce(_movePower, ForceMode.VelocityChange);
        _movePower = Vector3.zero;
        
        //重力を作る
        var boxCast = Physics.BoxCast(transform.position, transform.localScale/2, Vector3.down,out RaycastHit hit, Quaternion.identity); 
        if(hit.distance < _gravityScaleChangePoint[0])
        {

        }
        
    }
}
