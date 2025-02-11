using UnityEngine;

public class EndQuest : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    SceneChangeManager s = new();
    public void ReturnBase()
    {
        s.ReturnBase();
    }
    public void EndSound()
    {
        _audioSource.Play();
    }
}
