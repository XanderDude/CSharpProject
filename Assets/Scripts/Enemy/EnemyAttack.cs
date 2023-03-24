using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    enum WeaponType { Projectile, Melee }; //The types of weapons that the player can switch to
    [SerializeField] private WeaponType enemyWeapon; //a reference to the weapon types
    [SerializeField] private GameObject projectilePrefab; //the projectile that the player fires for projectile weapon
    [SerializeField] private Transform weaponAttackSpawn; //where each weapon will spawn its damaging colliders (projectiles for guns)
    [SerializeField] private int weaponDamage;
    [SerializeField] private float attackSpeed;
    private bool fired = false;
    [SerializeField] private ObjectPool pool;

    public void DoAttack()
    {

        switch (enemyWeapon)
        {
            case WeaponType.Projectile:
                GameObject projectileObj;
                if (!fired)
                {
                    projectileObj = pool.GetObject(weaponAttackSpawn.position, transform.rotation, null);
                    if (projectileObj.TryGetComponent<WeaponProjectile>(out WeaponProjectile projectile)) //if projectile has the weapon projectile script, grab it
                    {
                        projectile.InitDamage(weaponDamage, 0); //assign the projectile's damamge
                    }
                    StartCoroutine(AttackSpeed());
                }
                break;
        }
    }
    private IEnumerator AttackSpeed()
    {
        fired = true;
        yield return new WaitForSeconds(attackSpeed); //wait before next attack
        fired = false;
    }

}