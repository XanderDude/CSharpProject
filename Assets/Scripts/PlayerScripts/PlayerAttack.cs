using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerHUD pHUD; //the hud script that handles updating hud elements
    private GameManager manager;
    private Transform cameraTrans; //pos and rotation of main camera

    enum WeaponType { MachineGun, RocketL, Sword }; //The types of weapons that the player can switch to
    [SerializeField] private WeaponType playerWeapon; //a reference to the weapon types
    //enum WeaponStats
    [SerializeField] private WeaponBaseClass[] weapons;
    private WeaponBaseClass currentWeapon;

    [SerializeField] private GameObject projectilePrefab; //the projectile that the player fires for projectile weapon
    [SerializeField] private GameObject raycastTracerPrefab; //the projectile that'll visualize the raycast attack
    [SerializeField] private Transform weaponAttackSpawn; //where each weapon will spawn its damaging colliders (projectiles for guns)

    [SerializeField] private bool infiniteAmmo = false;

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = weapons[(int)playerWeapon];
        cameraTrans = Camera.main.transform;
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!manager.Paused)
        {

            if (Input.GetMouseButton(0)) //If shoot button inputed, the fired cooldown has ended, and it's not reloading
            {
                bool isRaycast = Physics.Raycast(cameraTrans.position, cameraTrans.forward, out RaycastHit hit); //shoot a ray from the camera forward, setting bool if it hits something
                currentWeapon.Shoot(isRaycast, hit, cameraTrans);
            }
            if (Input.GetKeyDown(KeyCode.R))//if player wants to reload
            {

                currentWeapon.Reload();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && playerWeapon != WeaponType.MachineGun)
            {
                currentWeapon = weapons[(int)WeaponType.MachineGun];
                playerWeapon = WeaponType.MachineGun;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && playerWeapon != WeaponType.RocketL)
            {
                currentWeapon = weapons[(int)WeaponType.RocketL];
                playerWeapon = WeaponType.RocketL;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && playerWeapon != WeaponType.Sword)
            {
                currentWeapon = weapons[(int)WeaponType.Sword];
                playerWeapon = WeaponType.Sword;
                SwitchWeapon();
            }

        }
    }
    
    private void SwitchWeapon()

    {
        foreach (WeaponBaseClass weapon in weapons)
        {
            if (weapon != currentWeapon)
            {
                weapon.gameObject.SetActive(false);
            }
            else
            {
                weapon.gameObject.SetActive(true);
            }
        }
    }

    private void Projectile()
    {
        bool isRaycast = Physics.Raycast(cameraTrans.position, cameraTrans.forward, out RaycastHit hit); //shoot a ray from the camera forward, setting bool if it hits something

        //Gun.Shoot(isRaycast, hit)
        switch (playerWeapon)
        {

            case WeaponType.RocketL:

                GameObject projectileObj;
                if (isRaycast)
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
                break;

            case WeaponType.Sword: break;
        }
    }
}
