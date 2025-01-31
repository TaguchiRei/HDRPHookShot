using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AttackerEnemyController : EnemyBase
{

    [SerializeField] GameObject _eye;
    [SerializeField] Transform _muzzlePos;
    VisualEffect _bulletEfect;

    private void Start()
    {
        PlayerHead = GameObject.FindGameObjectWithTag("PlayerHead");
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
        if (!Leader)
        {
            Debug.Log("MemberShot");
        }
        Physics.Raycast(transform.position, (delayedPosition - _muzzlePos.position).normalized, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default", "Ground"));
        _bulletEfect.SetInt("BulletType", 1);
        _bulletEfect.SetVector3("StartPos", _muzzlePos.transform.position);
        if (hit.collider.CompareTag("PlayerHead"))
        {
            _bulletEfect.SetVector3("EndVector", (hit.point - _muzzlePos.position) * 10000);
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
