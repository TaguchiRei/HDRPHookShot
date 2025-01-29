using UnityEngine;

public class EnemyEye : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PlayerHead");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform.position);
    }
}
