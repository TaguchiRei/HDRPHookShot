using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Story0TextManager : TextManager
{
    [SerializeField] GameObject _targetGroup;
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] List<GameObject> _wallObjects;
    List<GameObject> _targetGroupList = new();
    public bool CheckNext = false;
    public Vector3 checkPoint = Vector3.zero;
    IEnumerator coroutine;
    int phase = 0;
    private void Start()
    {
        coroutine = Tutorial();
        StartCoroutine(NextText(ShotTutorial, 0));
    }

    private void Update()
    {
        if (_targetGroupList.Count(a => a == null) == 4 && textPhase == 1)
        {
            textPhase = 2;
            coroutine.MoveNext();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            coroutine.MoveNext();
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

    IEnumerator<int> Tutorial()
    {
        _wallObjects[0].SetActive(false);
        _wallObjects.RemoveAt(0);
        yield return 0;
        StartCoroutine(enemyManager.ButtleStart(0));
        yield return 1;
        yield return 2;
        yield return 3;
        yield return 4;
    }
}
