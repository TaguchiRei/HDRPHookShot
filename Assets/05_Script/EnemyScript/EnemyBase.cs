using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public float MoveSpeed = 1;
    public NavMeshAgent Agent;
    public bool Leader = false;
    public Vector3 MoveEller = new(5, 5, 5);
    public EnemyStatus enemyStatus;
    public virtual void Move(Vector3 position)
    {
        Agent.speed = MoveSpeed;
        if (enemyStatus.Leader)
        {
            Agent.SetDestination(position);
        }
        else
        {
            Agent.SetDestination(position + new Vector3(Random.Range(MoveEller.x * -1, MoveEller.x), 0, Random.Range(MoveEller.z * -1, MoveEller.z)));
        }
    }
    public virtual void Stop()
    {
        Agent.SetDestination(transform.position);
    }
    public abstract void UniqueAction();
}