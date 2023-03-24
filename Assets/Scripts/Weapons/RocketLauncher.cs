using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : WeaponBaseClass
{
    //private bool infiniteAmmo = false;
    //public override event Action<int, int> OnPlayerShoot; //Publisher: action for updating hud when shooting, mag capacity and reserve
    public override void Shoot(bool raycastHit, RaycastHit hit, Transform cameraTrans)
    {
        if (reloading || fired)
        {
            return;
        }
        if (currentMagAmmo <= 0) //if mag empty, not reloading, or not just fired
        {
            if (CurrentReserveAmmo <= 0)// if reserve is also empty
            {
                return; //do nothing
            }
            StartCoroutine(ReloadDelay()); //wait for reloading to finish, then run ReloadMag()
            return; //cannot fire weapon so exit function
        }
        fired = true;
        //StartCoroutine(FireRate());
        GameObject projectileObj;
        if (raycastHit)
        {
            Debug.Log("Firing Projectile at Enemy");
            Vector3 vector = hit.point - weaponAttackSpawn.position; //Gets the direction to the point the player wants to fire the projectile
            Quaternion rotationVect = Quaternion.LookRotation(vector);
            projectileObj = objectPool.GetObject(weaponAttackSpawn.position, rotationVect, null);
        }
        else
        {
            projectileObj = objectPool.GetObject(weaponAttackSpawn.position, cameraTrans.rotation, null); //forward in the direction the camera is looking

        }
        if (projectileObj.TryGetComponent<WeaponProjectile>(out WeaponProjectile projectile)) //if projectile has the weapon projectile script, grab it
        {
            projectile.InitDamage(damage, pushBack); //assign the projectile's damamge
        }
        if (!infiniteAmmo)
        {
            CurrentMagAmmo = -1; //if infinite ammo is off, subract one from mag (Fires the gun)
            //OnPlayerShoot?.Invoke(currentMagAmmo, CurrentReserveAmmo); //invoke weapon reloading event if it exists
        }
    }

}
