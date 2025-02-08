using UnityEngine;

public class Story0TextManager : TextManager
{
    public override void Start()
    {
        base.Start();
        StartCoroutine(NextText(ShotTutorial));
    }

    void ShotTutorial()
    {

    }
}
