using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] PlayerInputSystem _playerInputSystem;
    [SerializeField] PlayerMove _playerMove;
    [SerializeField] GameObject _soundObject;
    [SerializeField] AudioClip[] _clips;
    [SerializeField] float audioLate = 0;
    

    void RailGunShotted()
    {
        _anim.SetBool("RailGunMode", false);
        _anim.SetBool("R_Shot", false);
        _playerInputSystem.ModeReset();
    }
    void TypeSound()
    {
        var sound = Instantiate(_soundObject);
        var audioS = sound.GetComponent<AudioSource>();
        audioS.clip = _clips[0];
        audioS.time = audioLate;
        audioS.Play();
    }
    public void UseAudioObject(AudioClip audioClip)
    {
        var sound = Instantiate(_soundObject);
        var audioS = sound.GetComponent<AudioSource>();
        audioS.clip = audioClip;
        audioS.time = 0.1f;
        audioS.Play();
    }
    void UsedAbility()
    {
        _playerMove.CanAction = true;
        _anim.SetBool("UseAbility", false);
    }
    void AbilityEffect(int abilityNumber)
    {
        switch (abilityNumber)
        {
            case 0:
                _playerMove.GaugeChanger(-45);
                break;
            case 1:

                break;
            default:
                break;
        }
    }
}
