using GamesKeystoneFramework.TextSystem;
using System;
using System.Collections;
using UnityEngine;

public abstract class TextManager : TextManagerBase
{
    [SerializeField] string[] _textDataName;
    int textPhase = 0;
    public virtual void Start()
    {
        StartText(_textDataName[0],true);
    }

    public IEnumerator NextText(Action action)
    {
        while (true)
        {
            var next = Next();
            yield return new WaitForSeconds(2.0f);
            if (!next)
                break;
        }
        action();
    }
}
