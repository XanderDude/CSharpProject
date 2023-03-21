using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : WeaponBaseClass
{
    
    public void Shoot(bool raycastHit, RaycastHit hit, Transform cameraTrans)
    {
        GameObject projectileObj;
        if (raycastHit)
        {
            Vector3 vector = hit.point - weaponAttackSpawn.position; //Gets the direction to the point the player wants to fire the projectile
            Quaternion rotationVect = Quaternion.LookRotation(vector);
            projectileObj = Instantiate(projectilePrefab, weaponAttackSpawn.position, rotationVect);

        }
        else
        {
            projectileObj = Instantiate(projectilePrefab, weaponAttackSpawn.position, cameraTrans.rotation); //forward in the direction the camera is looking
        }
        if (projectileObj.TryGetComponent<WeaponProjectile>(out WeaponProjectile projectile)) //if projectile has the weapon projectile script, grab it
        {
            //projectile.InitDamage(weaponDamage); //assign the projectile's damamge
        }
    }

}
