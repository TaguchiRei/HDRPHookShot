using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyManager : MonoBehaviour
{
    [SerializeField] public float _hp = 0;
    [SerializeField] public float _moveSpeed = 1;
    [SerializeField] public NavMeshAgent _agent;
    public virtual void Move(Vector3 position)
    {
        _agent.speed = _moveSpeed;
        _agent.SetDestination(position);
    }

    public abstract void UniqueAction();
}