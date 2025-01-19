using GamesKeystoneFramework.Core;
using GamesKeystoneFramework.SaveSystem.singleSave;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : SingleSaveBase
{
    public override List<Func<List<Data>>> CalledData { get; set; }
    public override List<Data> DataContents { get; set; }

    private void Awake()
    {
        DataContents = new();
        CalledData = new();
    }
}

/*
public class Test : MonoBehaviour, IHaveSaveData<Data>
{
    public List<Data> DataContents { get; set; }
    SaveDataManager _saveDataManager;
    void Start()
    {
        _saveDataManager = GameManager.Instance.GetComponent<SaveDataManager>();
        _saveDataManager.CalledData.Add(Send);
    }
    public List<Data> Send()
    {
        return DataContents;
    }
}
*/