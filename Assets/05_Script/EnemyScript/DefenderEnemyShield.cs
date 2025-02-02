using UnityEngine;

public class DefenderEnemyShield : MonoBehaviour
{
    private int _hp = 0;
    
    [SerializeField] int _maxHp = 5;
    [SerializeField] Animator _anim;
    [SerializeField] DefenderEnemyController _controller;
    float animFloat = 0f;
    public void SummonShield()
    {
        gameObject.SetActive(true);
        _hp = _maxHp;
        _anim.SetFloat("hit",0);
    }
    public void TakeAwayShield()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(animFloat > 0f)
        {
            _anim.SetFloat("hit",animFloat);
            animFloat -= Time.deltaTime;
        }
    }
    public void HPChanger(int dmg)
    {
        _hp -= dmg;
        animFloat = 0.5f;
        if (_hp <= 0)
        {
            gameObject.SetActive(false);
            _controller.BreakShield();
        }
    }
}
