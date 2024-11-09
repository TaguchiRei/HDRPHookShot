using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<MonoBehaviour>
{
    /// <summary>
    /// カメラの水平方向の反転の可否を行う。単に数値を下げればカメラの感度が変わる
    /// </summary>
    [Range(-1, 1)] public int _horiaontalCamera = 0;
    /// <summary>
    /// カメラの垂直方向の反転の可否を行う。単に数値を下げればカメラの感度が変わる
    /// </summary>
    [Range(-1, 1)] public int _verticalCamera = 0;

    void Start()
    {
        //マウスを固定して隠す
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

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

    public float Power { get => _power; set => _power = value; }
    public float BulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }
    public float RateOfFire { get => _rateOfFire; set => _rateOfFire = value; }
    public float MagazineCapacity { get => _magazineCapacity; set => _magazineCapacity = value; }
    public float RailgunPower { get => _railgunPower; set => _railgunPower = value; }
    public float RailgunAttackRange { get => _railgunAttackRange; set => _railgunAttackRange = value; }
}