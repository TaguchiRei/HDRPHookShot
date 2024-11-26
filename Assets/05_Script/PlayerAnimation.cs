using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _soundObject;
    [SerializeField] AudioClip[] _clips;
    [SerializeField] float audioLate = 0;
    void RailGunShotted()
    {
        _anim.SetBool("RailGunMode", false);
        _anim.SetBool("R_Shot", false);
        _player.GetComponent<PlayerInputSystem>().ModeReset();
    }
    void TypeSound()
    {
        var sound = Instantiate(_soundObject);
        var audioS = sound.GetComponent<AudioSource>();
        audioS.clip = _clips[0];
        audioS.time = audioLate;
        audioS.Play();
    }
}
