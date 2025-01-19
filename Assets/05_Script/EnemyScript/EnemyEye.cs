using UnityEngine;

public class EnemyEye : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);
    }
}
