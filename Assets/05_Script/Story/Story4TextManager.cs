using TMPro;
using UnityEngine;

public class Story4TextManager : TextManager
{
    [SerializeField] Animator _clearAnimator;
    [SerializeField] TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] float _maxMin = 0;
    float _time = 0;
    bool _start = false;
    bool timerStop;
    bool _stopped = false;
    private void Start()
    {
        StartCoroutine(NextText(ButtleStart, 0));
    }
    private void Update()
    {
        if (_start)
        {
            float remainingTime = _maxMin + _time - Time.time;
            if (remainingTime <= 0)
            {
                _textMeshProUGUI.text = "����";
                _clearAnimator.SetBool("Clear", true);
            }
            else if(!timerStop)
            {
                _time -= enemyManager._measurementNum / 300;
                enemyManager._measurementNum = 0;
                _textMeshProUGUI.text = $"�����܂�{(int)(remainingTime / 60):D2}��{(int)(remainingTime % 60):D2}�b";
            }
            else
            {
                _time += Time.deltaTime;
                _textMeshProUGUI.text = $"�����܂�{(int)(remainingTime / 60):D2}��{(int)(remainingTime % 60):D2}�b";
            }

            if(!_stopped &&_maxMin / 3 > remainingTime)
            {
                _stopped = true;
            }
        }
    }
    void ButtleStart()
    {
        StartCoroutine(enemyManager.ButtleStart(4));
        enemyManager._measurement = true;
        StartCoroutine(NextText(None, 1));
        _start = true;
        _time = Time.time;
    }
    void None()
    {

    }
    void TimerStop()
    {
        timerStop = true;
    }
}
