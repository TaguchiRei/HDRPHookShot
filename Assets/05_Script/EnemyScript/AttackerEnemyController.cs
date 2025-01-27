using UnityEngine;

public class AttackerEnemyController : EnemyBase
{

    [SerializeField] GameObject _eye;


    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("PlayerHead");
        Debug.Log("enemyStart");
        timer = 5;
    }
    void Update()
    {
        if (survive)
        {
            Physics.Raycast(transform.position, (Player.transform.position - transform.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground"));

            if (Agent.remainingDistance <= 0.5f && !Agent.hasPath)
            {
                Animator.SetBool("Walking", false);
            }
            else
            {
                Animator.SetBool("Walking", true);
            }

            if (enemyStatus.Leader)
            {

            }
        }
    }
    public override void UniqueAction()
    {

    }


    public override void Move(Vector3 position)
    {
        base.Move(position);
        Animator.SetBool("Walking", true);
    }
}
