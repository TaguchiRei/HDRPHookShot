using System.Collections.Generic;
using UnityEngine;

public class Story0TextManager : TextManager
{
    [SerializeField] GameObject targetGroup;
    List<GameObject> targetGroupList = new();
    public override void Start()
    {
        base.Start();
        StartCoroutine(NextText(ShotTutorial));
    }

    private void Update()
    {
        switch (textPhase)
        {
            case 0:
                break;
            case 1:

                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

    void ShotTutorial()
    {
        var targetGroupInstance = Instantiate(targetGroup);
        foreach (Transform child in targetGroupInstance.transform)
        {
            targetGroupList.Add(child.gameObject);
        }
        textPhase = 1;
    }
}
