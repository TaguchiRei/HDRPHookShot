using GamesKeystoneFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<MonoBehaviour>, IHaveSaveData<Data>
{
    /// <summary>
    /// カメラの水平方向の反転の可否を行う。単に数値を下げればカメラの感度が変わる
    /// </summary>
    [Range(-1, 1)] public int _horizontalCamera = 1;
    /// <summary>
    /// カメラの垂直方向の反転の可否を行う。単に数値を下げればカメラの感度が変わる
    /// </summary>
    [Range(-1, 1)] public int _verticalCamera = 1;

    public bool _pause = false;
    public string _playerName = string.Empty;
    public Action InButtlePause;
    public Action InButtleReStart;
    [SerializeField] SaveDataManager _saveDataManager;

    public List<Data> DataContents { get; set; }

    void Start()
    {
        //マウスを固定して隠す
        DataContents = new List<Data>();
        Cursor.lockState = CursorLockMode.Locked;
        _saveDataManager.CalledData.Add(Send);
    }

    public List<Data> Send()
    {
        return DataContents;
    }
    //メニューを開くときのプログラム
    public void Stop()
    {
        InButtlePause.Invoke();
        _pause = true;
    }

    public void OpenMenu()
    {

    }

    public void ReStart()
    {
        InButtleReStart.Invoke();
        _pause = false;
    }

    public override void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        switch (s.name)
        {
            case "Start":
                break;
            case "Bace":
                break;
            case "Buttle":
                break;
            default:
                break;

        }
    }

    void ButtleSceneStart(int sceneNumber)
    {

    }
}


[Serializable]
public class WeaponStatus
{
    [SerializeField] float _power = 1f;
    [SerializeField] float _bulletSpeed = 10f;
    [SerializeField] float _rateOfFire = 1f;
    [SerializeField] float _magazineCapacity = 10f;
    [SerializeField] float _railgunPower = 5f;
    [SerializeField] float _railgunAttackRange = 5f;
    [SerializeField] float _hookShotSpeed = 50f;
    [SerializeField] float _hookShotTimer = 5f;

    public float Power { get => _power; set => _power = value; }
    public float BulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }
    public float RateOfFire { get => _rateOfFire; set => _rateOfFire = value; }
    public float MagazineCapacity { get => _magazineCapacity; set => _magazineCapacity = value; }
    public float RailgunPower { get => _railgunPower; set => _railgunPower = value; }
    public float RailgunAttackRange { get => _railgunAttackRange; set => _railgunAttackRange = value; }
    public float HookShotSpeed { get => _hookShotSpeed; set => _hookShotSpeed = value; }
    public float HookShotTimer { get => _hookShotTimer; set => _hookShotTimer = value; }
}