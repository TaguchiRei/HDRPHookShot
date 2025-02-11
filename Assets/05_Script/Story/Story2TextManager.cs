using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Story2TextManager : TextManager
{
    List<PipeScript> _pipes = new();
    [SerializeField] List<int> _knockDownNum = new();
    [SerializeField] Animator _clearAnimator;
    IEnumerator Quest;
    int _knockDown = 0;
    public int Progress = 0;

    private void Start()
    {
        enemyManager._measurement = true;
        StartCoroutine(NextText(ButtleStart, 0));
        _pipes = FindObjectsByType<PipeScript>(default).ToList();
        _knockDown = _knockDownNum.Count -1;
    }
    private void Update()
    {
        if (_knockDownNum.Count != 0 && enemyManager._measurementNum >= _knockDownNum[0])
        {
            enemyManager._measurementNum = 0;
            var r = UnityEngine.Random.Range(0, _knockDownNum.Count);
            _pipes[r].PipeActivate();
            _pipes[r].TextManager = this;
            _knockDownNum.RemoveAt(r);
        }
        if (_knockDownNum.Count == 0 && phase == _knockDown)
        {
            _clearAnimator.SetBool("Clear", true);
        }

    }

    void ButtleStart()
    {
        StartCoroutine(enemyManager.ButtleStart(2));
        StartCoroutine(NextText(None, 1));
    }
    void None()
    {

    }
}
