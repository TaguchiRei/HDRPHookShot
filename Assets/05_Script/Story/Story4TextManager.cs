using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Story4TextManager : TextManager
{
    [SerializeField] Animator _clearAnimator;
    [SerializeField] TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] float _maxMin = 0;
    [SerializeField] int _errorPipe = 3;
    List<PipeScript> _pipeList = new();
    float _time = 0;
    bool _start = false;
    bool timerStop;
    bool _stopped = false;
    private void Start()
    {
        _pipeList = FindObjectsByType<PipeScript>(default).ToList();
        StartCoroutine(NextText(ButtleStart, 0));
    }
    private void Update()
    {
        if (_start)
        {
            float remainingTime = _maxMin + _time - Time.time;
            if (remainingTime <= 0)
            {
                _textMeshProUGUI.text = "Š®—¹";
                _clearAnimator.SetBool("Clear", true);
            }
            else if (!timerStop)
            {
                _time -= enemyManager._measurementNum / 300;
                enemyManager._measurementNum = 0;
                _textMeshProUGUI.text = $"Š®—¹‚Ü‚Å{(int)(remainingTime / 60):D2}•ª{(int)(remainingTime % 60):D2}•b";
            }
            else
            {
                _time += Time.deltaTime;
                _textMeshProUGUI.text = $"Š®—¹‚Ü‚Å{(int)(remainingTime / 60):D2}•ª{(int)(remainingTime % 60):D2}•b";
            }

            if (!_stopped && _maxMin / 3 > remainingTime)
            {
                _stopped = true;
                TimerStop();
            }
            if (_stopped && phase >= _errorPipe)
            {
                timerStop = false;
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
        NextText(PipeChange, 2);
    }
    void PipeChange()
    {
        for (int i = 0; i < _errorPipe; i++)
        {
            var r = Random.Range(0, _pipeList.Count);
            _pipeList[r].PipeActivate();
            _pipeList.RemoveAt(r);
        }
    }
}
