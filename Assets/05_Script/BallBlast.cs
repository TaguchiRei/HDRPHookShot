using UnityEngine;

public class BallBlast : MonoBehaviour
{
    public void BlastAttack(float radius)
    {
        Collider[] attack =  Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in attack)
        {
            switch (c.gameObject.tag)
            {
                case "Enemy":
                    c.gameObject.GetComponent<EnemyStatus>().HPChanger(1);
                    break;
                case "Player":
                    var pm = c.gameObject.GetComponent<PlayerMove>();
                    pm.GaugeChanger(32);
                    pm.GaugeChanger(12,false);
                    break;
                case "Barrier":
                    c.GetComponent<DefenderEnemyShield>().HPChanger(10);
                    break;
                default:
                    break;
            }
        }
    }
    public void BlastDestroy()
    {
        Destroy(gameObject);
    }
}
