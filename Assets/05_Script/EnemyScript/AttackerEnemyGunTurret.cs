using GamesKeystoneFramework.PolarCoordinates;
using System.Collections.Generic;
using UnityEngine;

public class AttackerEnemyGun : MonoBehaviour
{
    Transform player;
    [SerializeField] float delay = 0.5f;
    private readonly Queue<Vector3> positionHistory = new();
    private float elapsedTime = 0f;
    [SerializeField] bool x = false;
    Vector3 delayedPosition;
    Vector3 direction;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHead").GetComponent<Transform>();
    }
    void Update()
    {
        positionHistory.Enqueue(player.position);
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= delay)
        {
            if (!x)
            {
                //Y軸を回転させる。delay秒前の位置を追従
                delayedPosition = positionHistory.Dequeue();
                direction = delayedPosition - transform.position;
                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            else
            {
                //極座標を用いて見るべきプレイヤーの位置を調べる。
                delayedPosition = positionHistory.Dequeue();
            }
        }
    }
}


