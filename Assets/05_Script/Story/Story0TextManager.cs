using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Story0TextManager : TextManager
{
    [SerializeField] GameObject _targetGroup;
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] List<GameObject> _wallObjects;
    [SerializeField] Animator _animator;
    List<GameObject> _targetGroupList = new();
    public bool CheckNext = false;
    public Vector3 checkPoint = Vector3.zero;
    IEnumerator coroutine;
    private void Start()
    {
        coroutine = Tutorial();
        StartCoroutine(NextText(ShotTutorial, 0));
        PlayerMove.GaugeChanger(30);
    }

    private void Update()
    {
        if (_targetGroupList.Count(a => a == null) == 4 && textPhase == 1)
        {
            textPhase = 2;
            coroutine.MoveNext();
        }
        if(textPhase == 2 && enemyManager._measurementNum >= 9)
        {
            PlayerMove._invincible = true;
            var pd = FindAnyObjectByType<PlayerData>();
            if (pd.SaveData.Progress == 0)
            {
                pd.SaveData.Progress = 1;
                pd.SaveData.Save(0);
            }
            _animator.SetBool("Clear",true);
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
        enemyManager._measurement = true;
        StartCoroutine(enemyManager.ButtleStart(0));
        yield return 1;
    }
}
