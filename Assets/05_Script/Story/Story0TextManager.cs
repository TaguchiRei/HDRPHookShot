using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Story0TextManager : TextManager
{
    [SerializeField] GameObject _targetGroup;
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] List<GameObject> _wallObjects;
    List<GameObject> _targetGroupList = new();
    public bool CheckNext = false;
    private void Start()
    {
        StartCoroutine(NextText(ShotTutorial,0));
    }

    private void Update()
    {
        switch (textPhase)
        {
            case 0:
                break;
            case 1:
                if (_targetGroupList.Count(a => a == null) == 4)
                {
                    textPhase = 0;
                    StartCoroutine(NextText(HookShotTutorial, 1));
                    _wallObjects[0].SetActive(false);
                    _wallObjects.RemoveAt(0);
                }
                
                break;
            case 2:
                if (CheckNext)
                {
                    textPhase = 0;
                    CheckNext = false;
                    StartCoroutine(NextText(HookShotTutorial,2));
                    _wallObjects[0].SetActive(false);
                    _wallObjects.RemoveAt(0);
                }
                break;
            case 3:
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckNext = true;
            _boxCollider.enabled = false;
        }
    }

    void ShotTutorial()
    {
        var targetGroupInstance = Instantiate(_targetGroup);
        foreach (Transform child in targetGroupInstance.transform)
        {
            _targetGroupList.Add(child.gameObject);
        }
        textPhase = 1;
    }
    void HookShotTutorial()
    {
        textPhase = 2;
    }
}
