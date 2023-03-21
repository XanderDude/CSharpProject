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

    [SerializeField] private int weaponDamage = 10;

    [SerializeField] private float fireRate = 0.5f; //time to wait till before next bullet can be fired
    private float fireRateTimer = 0; //the timer for checking when to fire again
    private bool fired = false; //cooldown for checking if the gun has just been fired. Once fire, start counting and prevent continued firing
    [SerializeField] private float reloadSpeed = 2f; //time it takes to refill mag ammo when reloading
    private bool reloading = false; //a check for if the gun is currently reloading

    //Weapon 1: Machine Gun
    //Raycast
    //[SerializeField] private int MGDamage = 10;
    //[SerializeField] private float MGFireRate = 0.5f; //time to wait till before next bullet can be fired
    //[SerializeField] private float MGReloadSpeed = 2f; //time it takes to refill mag ammo when reloading
    //[SerializeField] private int MGMagSize = 50;
    //private int MGCurrentMag;
    //[SerializeField] private int MGReserveSize = 200;
    //private int MGCurrentReserve;

    ////Weapon 2: Rocket Launcher
    ////Projectile
    //[SerializeField] private int RLDamage = 34;
    //[SerializeField] private float RLFireRate = 2f; //time to wait till before next rocket can be fired
    //[SerializeField] private float RLReloadSpeed = 3f; //time it takes to refill mag ammo when reloading
    //[SerializeField] private int RLMagSize = 10;
    //private int RLCurrentMag;
    //[SerializeField] private int RLReserveSize = 50;
    //private int RLCurrentReserve;

    ////Weapon 3: Sword
    ////Animation
    //[SerializeField] private int SWDamage = 50;
    //[SerializeField] private float SWFireRate = 1f; //time to wait till before next swing
    //[SerializeField] private float SWReloadSpeed = 0f; 

    [SerializeField] private bool infiniteAmmo = false;

    private int ammoCurrentMag = 50; //starting ammo in the magazine
    [SerializeField] private int ammoMaxMagSize = 50; //max ammo allowed in magazine
    public int AmmoCurrentMag //public property for the player's ammo count in the magazine (reloading/shooting)
    {
        get {
            switch (playerWeapon)
            {

                case WeaponType.RocketL:
                    return ammoCurrentMag;
                case WeaponType.Sword:
                    return ammoCurrentMag;
                default:
                    return ammoCurrentMag;
            }
        }
        set
        {
            if (ammoCurrentMag == 0 && value >= ammoMaxMagSize) //a full reload from zero will exclude the round in the chamber
            {
                ammoCurrentMag = ammoMaxMagSize - 1; //full reload minus round in chamber
            }
            else if (ammoCurrentMag + value > ammoMaxMagSize) //ammo increases past max
            {
                ammoCurrentMag = ammoMaxMagSize; //ammo in mag cannot go past max (no extra in the chamber)
            }
            else if (ammoCurrentMag + value < 0) //ammo in mag depleted
            {
                ammoCurrentMag = 0; //stay at zero ammo
            }
            else
            {
                ammoCurrentMag += value; //adjust ammo
            }

            pHUD.HUDUpdateAmmo(AmmoCurrentMag, AmmoCurrentReserve); //update hud with current ammo in mag and reserve
        }
    }

    private int ammoCurrentReserve = 100; //starting amount of reserve ammo
    [SerializeField] private int ammoMaxReserve = 200; //max ammo allowed in reserve
    public int AmmoCurrentReserve     //property handling ammo in reserve (not in gun/pickups/pulling from when reloading)
    {
        get { return ammoCurrentReserve; }
        set
        {
            int ammoGoneInMag = ammoMaxMagSize - ammoCurrentMag; //the amount of ammo missing in the mag
            //Debug.Log(value);
            if (ammoCurrentReserve + value > ammoMaxReserve + ammoGoneInMag) //when picking up ammo, if pickup exceeds max reserve plus ammo missing in mag..
            {
                ammoCurrentReserve = ammoMaxReserve + ammoGoneInMag; //..set to max reserve ammo plus what's missing in mag

            }
            else if (ammoCurrentReserve + value < 0) //when reloading, if reserve goes below zero
            {
                ammoCurrentReserve = 0; //prevent from going below zero

            }
            else
            {
                ammoCurrentReserve += value; //adjust ammo in reserve

            }
            //Debug.Log(ammoCurrentReserve);
            pHUD.HUDUpdateAmmo(AmmoCurrentMag, AmmoCurrentReserve); //update hud with current ammo in mag and reserve
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = weapons[(int)playerWeapon];
        cameraTrans = Camera.main.transform;
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        pHUD.HUDUpdateAmmo(AmmoCurrentMag, AmmoCurrentReserve); //call to update hud
        ammoMaxMagSize += 1; //allow for a round in the chamber
    }

    // Update is called once per frame
    void Update()
    {
        if (!manager.Paused)
        {
            //if (fired) //gun firing cooldown has started
            //{
            //    FireRate(); //calculate firerate for when gun can be fired again
            //}
            if (Input.GetMouseButton(0)) //If shoot button inputed, the fired cooldown has ended, and it's not reloading
            {
                bool isRaycast = Physics.Raycast(cameraTrans.position, cameraTrans.forward, out RaycastHit hit); //shoot a ray from the camera forward, setting bool if it hits something
                currentWeapon.Shoot(isRaycast, hit);
                //FireWeapon(); //attempt to deal damage by shooting/attacking
            }
            if (Input.GetKeyDown(KeyCode.R) && ammoCurrentReserve > 0 && !reloading)// && ammoCurrentMag != ammoMaxMagSize) //if player wants to reload and reserve is not empty
            {
                reloading = true; //reloading has started
                StartCoroutine(ReloadDelay()); //wait for reloading to finish, then run ReloadMag()

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


    

    private void FireWeapon()
    {
        if (ammoCurrentMag <= 0) //if mag empty
        {
            if (ammoCurrentReserve <= 0)// if reserve is also empty
            {
                return; //do nothing
            }
            reloading = true; //reloading has started
            StartCoroutine(ReloadDelay()); //wait for reloading to finish, then run ReloadMag()
            return; //cannot fire weapon so exit function
        }
        Projectile(); //attack with projectile, or the weapons specific attack type
        if (!infiniteAmmo) {
            AmmoCurrentMag = -1; //if infinite ammo is off, subract one from mag (Fires the gun)
        }
        fired = true; //start fired cooldown
    }

    private IEnumerator ReloadDelay()
    {
        pHUD.HUDToggleReloading(reloading); //turn on reloading text
        yield return new WaitForSeconds(reloadSpeed); //wait according to reload speed
        ReloadMag(); //then add ammo to mag
        reloading = false; //the reload has completed
        pHUD.HUDToggleReloading(reloading); //turn off reloading text
    }

    private void ReloadMag()
    {
        //This ensures reserve ammo only loses what's needed to refill mag, and the mag only gets what's available in reserve
        //Also checks if mag is empty to prevent magically reloading round in chamber
        int ammoNeeded = ammoCurrentMag == 0 ? //To calculate ammo needed to fill mag: if mag is empty..
            (ammoMaxMagSize - 1) - ammoCurrentMag : //..ammo needed is max minus round in chamber, else..
            ammoMaxMagSize - ammoCurrentMag; //..the ammo needed is max minus what's in mag
        AmmoCurrentMag = ammoCurrentReserve; //add what's in reserve to mag. Adding reserve for when reserve is not enough to fill mag to max
        AmmoCurrentReserve = -(ammoNeeded); //subtract from reserve what's needed to fill mag.
    }

    private void FireRate() //called in update when gun is fired
    {
        fireRateTimer += Time.deltaTime; //counting up from 0
        if (fireRateTimer >= fireRate) //timer has reached wait time for firing next shot
        {
            fireRateTimer = 0; //reset timer
            fired = false;  //end cooldown for firing, stop FireRate() from being called
        }
    }

    private void Projectile()
    {
        bool isRaycast = Physics.Raycast(cameraTrans.position, cameraTrans.forward, out RaycastHit hit); //shoot a ray from the camera forward, setting bool if it hits something

        //Gun.Shoot(isRaycast, hit)
        switch (playerWeapon)
        {
            case WeaponType.MachineGun:
                if (isRaycast) //if the ray hit something
                {
                    if (hit.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy)) //if raycast hits, try to grab enemy health
                    {
                        enemy.Damage(weaponDamage); //pass damage to enemy Damage function
                    }
                }
                break;
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
                    projectile.InitDamage(weaponDamage); //assign the projectile's damamge
                }
                break;

            case WeaponType.Sword: break;
        }
    }
}
