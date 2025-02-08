using GamesKeystoneFramework.TextSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextManager : TextManagerBase
{
    public string[] _textDataName;
    public int textPhase = 0;

    public IEnumerator NextText(Action action,int _textDataNum)
    {
        StartText(_textDataName[0], true);
        while (true)
        {
            var next = Next();
            yield return new WaitForSeconds(1.0f);
            if (!next)
                break;
        }
        action();
    }
}
