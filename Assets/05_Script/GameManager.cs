using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<MonoBehaviour>
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

    public Action InButtlePause;
    public Action InButtleReStart;


    void Start()
    {
        //マウスを固定して隠す
        Cursor.lockState = CursorLockMode.Locked;
    }

    //メニューを開くときの
    public void Stop()
    {
        InButtlePause.Invoke();
        _pause = true;
    }

    public void ReStart()
    {
        InButtleReStart.Invoke();
        _pause = false;
    }

    public override void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        base.OnSceneLoaded(s, mode);
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