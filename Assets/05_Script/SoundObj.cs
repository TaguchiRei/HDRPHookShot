using UnityEngine;

public class SoundObj : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
