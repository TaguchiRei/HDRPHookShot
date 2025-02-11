using GamesKeystoneFramework.TextSystem;
using System;
using System.Collections;
using UnityEngine;

public abstract class TextManager : TextManagerBase
{
    public string[] _textDataName;
    public int textPhase = 0;
    public EnemyManager enemyManager;
    public PlayerMove PlayerMove;

    public bool PhaseChange = true;

    public int phase = 0;

    public int Mode = 0;
    public IEnumerator NextText(Action action, int _textDataNum)
    {
        StartText(_textDataName[_textDataNum], true);
        while (true)
        {
            var next = Next();
            yield return new WaitForSeconds(2f);
            if (!next)
                break;
        }
        action();
    }
}
