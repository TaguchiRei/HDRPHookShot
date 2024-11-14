using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] GameObject _player;
    void RailGunShotted()
    {
        _anim.SetBool("RailGunMode", false);
        _anim.SetBool("R_Shot", false);
        _player.GetComponent<PlayerInputSystem>().ModeReset();
    }
}
