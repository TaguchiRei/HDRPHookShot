using UnityEngine;

public class Story1TextManager : TextManager
{
    [SerializeField] Animator _clearAnimator;
    [SerializeField] int maxNum = 150;
    bool _started = false;
    private void Start()
    {
        StartCoroutine(NextText(ButtleStart, 0));
    }

    private void Update()
    {
        if (_started && enemyManager._measurementNum >= maxNum)
        {
            PlayerMove._invincible = true;
            var pd = FindAnyObjectByType<PlayerData>();
            if (pd.SaveData.Progress == 1)
            {
                pd.SaveData.Progress = 2;
                pd.SaveData.Save(0);
            }
            _clearAnimator.SetBool("Clear", true);
        }
    }

    void ButtleStart()
    {
        StartCoroutine(enemyManager.ButtleStart(1));
        _started = true;
        enemyManager._measurement = true;
        StartCoroutine(NextText(None, 1));
    }
    void None()
    {

    }
}
