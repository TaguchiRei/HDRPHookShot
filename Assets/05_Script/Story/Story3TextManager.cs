using TMPro;
using UnityEngine;

public class Story3TextManager : TextManager
{
    [SerializeField] Animator _clearAnimator;

    [SerializeField] private int _maxDmg = 500;

    [SerializeField] TextMeshProUGUI _textMeshProUGUI;

    private void Start()
    {
        StartCoroutine(NextText(ButtleStart,0));
    }
    private void Update()
    {
        _textMeshProUGUI.text = "残り:" + Mathf.Max(0,_maxDmg - phase).ToString() + "ダメージ";
        if(phase >= _maxDmg &&  !PlayerMove._invincible)
        {
            PlayerMove._invincible = true;
            var pd = FindAnyObjectByType<PlayerData>();
            if (pd.SaveData.Progress == 3)
            {
                pd.SaveData.Progress = 4;
                pd.SaveData.Save(0);
            }
            _clearAnimator.SetBool("Clear", true);
        }
    }
    void ButtleStart()
    {
        StartCoroutine(enemyManager.ButtleStart(3));
        StartCoroutine(NextText(None,1));
    }
    void None()
    {

    }
}
