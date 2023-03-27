using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponBaseClass
{
    [SerializeField] private MeleeAttack blade;

    private void Start()
    {
        blade.InitDamage(damage, pushBack);
    }
    public override void Shoot(bool raycastHit, RaycastHit hit, Transform cameraTrans)
    {
        if (fired)
        {
            return;
        }
        fired = true;
        anim.SetTrigger("Shoot");
    }

    public override void Reload()
    {
        //do nothing
    }

}
