using UnityEngine;

public class Story0Wall : MonoBehaviour
{
    [SerializeField] Story0TextManager textManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.transform.position = textManager.checkPoint;
        }
    }
}
