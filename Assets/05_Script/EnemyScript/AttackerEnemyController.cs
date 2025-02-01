using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AttackerEnemyController : EnemyBase
{

    [SerializeField] GameObject _eye;
    [SerializeField] Transform _muzzlePos;
    VisualEffect _bulletEfect;
    public override void Start()
    {
        base.Start();
        _bulletEfect = GameObject.FindGameObjectWithTag("VFX").GetComponent<VisualEffect>();
        timer = 5;
    }
    public override void Update()
    {
        base.Update();
    }
    public override void UniqueInitialization()
    {

    }

    public override void UniqueAction(Vector3 delayedPosition)
    {
        Physics.Raycast(_muzzlePos.position, (delayedPosition - _muzzlePos.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground","PlayerHead"));
        _bulletEfect.SetInt("BulletType", 2);
        _bulletEfect.SetVector3("StartPos", _muzzlePos.transform.position);
        if (hit.collider != null && hit.collider.CompareTag("PlayerHead"))
        {
            _bulletEfect.SetVector3("EndVector", (hit.point - _muzzlePos.position) * 1000);
            PlayerMoveI.GaugeChanger(1);
        }
        else
        {
            _bulletEfect.SetVector3("EndVector", hit.point - _muzzlePos.position);
        }
        _bulletEfect.SendEvent("NomalBullet");
        
    }
    public override void Move(Vector3 position)
    {
        base.Move(position);
        Animator.SetBool("Walking", true);
    }
}
