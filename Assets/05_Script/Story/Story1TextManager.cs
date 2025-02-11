using UnityEngine;

public class Story1TextManager : TextManager
{
    [SerializeField] Animator _clearAnimator;
    bool _started = false;
    private void Start()
    {
        StartCoroutine(NextText(ButtleStart, 0));
    }

    private void Update()
    {
        if (_started && enemyManager._measurementNum == 150)
        {
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
