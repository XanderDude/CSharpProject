using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseClass : MonoBehaviour
{
    [SerializeField] protected Transform weaponAttackSpawn; //spawn for projectile
    [SerializeField] protected GameObject projectilePrefab; //the prefab projectile that's fired
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float fireRate = 0.5f; //time to wait till before next bullet can be fired
    [SerializeField] protected bool fired = false;
    private float fireRateTimer = 0f;
    [SerializeField] protected float reloadSpeed = 2f; //time it takes to refill mag ammo when reloading
    public bool reloading = false; //if currently reloading
    [SerializeField] protected int magSize = 50;
    protected int currentMagAmmo;
    [SerializeField] private Animator anim;
    public int CurrentMagAmmo //public property for the player's ammo count in the magazine (reloading/shooting)
    {
        get
        {
            return currentMagAmmo;
        }
        set
        {
            if (currentMagAmmo == 0 && value >= magSize) //a full reload from zero will exclude the round in the chamber
            {
                currentMagAmmo = magSize - 1; //full reload minus round in chamber
            }
            else if (currentMagAmmo + value > magSize) //ammo increases past max
            {
                currentMagAmmo = magSize; //ammo in mag cannot go past max (no extra in the chamber)
            }
            else if (currentMagAmmo + value < 0) //ammo in mag depleted
            {
                currentMagAmmo = 0; //stay at zero ammo
            }
            else
            {
                currentMagAmmo += value; //adjust ammo
            }
            OnPlayerShoot?.Invoke(CurrentMagAmmo, CurrentReserveAmmo); //invoke hud ammo counter to update if exists
        }
    }
    [SerializeField] protected int reserveSize = 200;
    private int currentReserveAmmo;
    public int CurrentReserveAmmo     //property handling ammo in reserve (not in gun/pickups/pulling from when reloading)
    {
        get { return currentReserveAmmo; }
        set
        {
            int ammoGoneInMag = magSize - currentMagAmmo; //the amount of ammo missing in the mag
            //Debug.Log(value);
            if (currentReserveAmmo + value > reserveSize + ammoGoneInMag) //when picking up ammo, if pickup exceeds max reserve plus ammo missing in mag..
            {
                currentReserveAmmo = reserveSize + ammoGoneInMag; //..set to max reserve ammo plus what's missing in mag

            }
            else if (currentReserveAmmo + value < 0) //when reloading, if reserve goes below zero
            {
                currentReserveAmmo = 0; //prevent from going below zero

            }
            else
            {
                currentReserveAmmo += value; //adjust ammo in reserve

            }
            OnPlayerShoot?.Invoke(CurrentMagAmmo, CurrentReserveAmmo); //invoke hud ammo counter to update if exists
        }
    }


    [SerializeField] protected ObjectPool objectPool;

    protected bool infiniteAmmo = false;

    public event Action<float, int, int> OnPlayerReload; //Publisher: action for showing the reloading hud notif. Reload time, current mag ammo, current reserve ammo.
    public virtual event Action<int, int> OnPlayerShoot; //Publisher: action for updating hud when shooting, mag capacity and reserve

    private void Start()
    {
        currentMagAmmo = magSize;
        currentReserveAmmo = reserveSize;
        magSize += 1;
        OnPlayerShoot?.Invoke(CurrentMagAmmo, CurrentReserveAmmo); //invoke hud ammo counter to update if exists
    }

    private void OnEnable()
    {
        if (GameObject.FindGameObjectWithTag("Player").TryGetComponent<PlayerInputs>(out PlayerInputs player))
        {
            infiniteAmmo = player.infiniteAmmo;
        }
        OnPlayerShoot?.Invoke(CurrentMagAmmo, CurrentReserveAmmo); //invoke hud ammo counter to update if exists
    }

    private void Update()
    {
        if (fired)
        {
            fireRateTimer += Time.deltaTime; //when fired start counting
        }
        if (fireRateTimer >= fireRate) //when timer reaches fire rate stop counting
        {
            fired = false;
            fireRateTimer = 0;
        }
    }

    public void Reload()
    {
        if (!reloading && currentReserveAmmo > 0 && currentMagAmmo != magSize)
        {
            Debug.Log("Start Reload");
            anim.SetTrigger("Reload");
            StartCoroutine(ReloadDelay());
        }
    }
    protected IEnumerator ReloadDelay()
    {
        OnPlayerReload?.Invoke(reloadSpeed, 0, currentReserveAmmo); //invoke weapon reloading event if it exists
        reloading = true;
        //pHUD.HUDToggleReloading(reloading); //turn on reloading text
        yield return new WaitForSeconds(reloadSpeed); //wait according to reload speed
        ReloadMag(); //then add ammo to mag

    }

    protected void ReloadMag()
    {
        //This ensures reserve ammo only loses what's needed to refill mag, and the mag only gets what's available in reserve
        //Also checks if mag is empty to prevent magically reloading round in chamber
        int ammoNeeded = currentMagAmmo == 0 ? //To calculate ammo needed to fill mag: if mag is empty..
            (magSize - 1) - currentMagAmmo : //..ammo needed is max minus round in chamber, else..
            magSize - currentMagAmmo; //..the ammo needed is max minus what's in mag
        CurrentMagAmmo = currentReserveAmmo; //add what's in reserve to mag. Adding reserve for when reserve is not enough to fill mag to max
        CurrentReserveAmmo = -(ammoNeeded); //subtract from reserve what's needed to fill mag.
        //OnPlayerShoot?.Invoke(currentMagAmmo, currentReserveAmmo); //update hud after reload completes
        reloading = false; //the reload has completed
    }

    public virtual void Shoot(bool raycastHit, RaycastHit hit, Transform cameraTrans)
    {
        if (reloading || fired)
        {
            return;
        }
        if (currentMagAmmo <= 0) //if mag empty, not reloading, or not just fired
        {
            if (currentReserveAmmo <= 0)// if reserve is also empty
            {
                return; //do nothing
            }
            StartCoroutine(ReloadDelay()); //wait for reloading to finish, then run ReloadMag()
            return; //cannot fire weapon so exit function
        }
        fired = true;
        //projectileObj = Instantiate(projectilePrefab, weaponAttackSpawn.position, cameraTrans.rotation); //forward in the direction the camera is looking
        objectPool.GetObject(weaponAttackSpawn.position, cameraTrans.rotation, null);
        if (raycastHit && hit.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy)) //if raycast hits, try to grab enemy health
        {
            enemy.Damage(damage); //pass damage to enemy Damage function
        }
        if (!infiniteAmmo)
        {
            CurrentMagAmmo = -1; //if infinite ammo is off, subract one from mag
        }
    }

    protected IEnumerator FireRate() //called in update when gun is fired
    {
        fired = true; //start fired cooldown
        yield return new WaitForSecondsRealtime(fireRate); 
        fired = false; //end fired cooldown
    }

    public void SwitchWeaponAnim()
    {
        anim.SetBool("WeaponEquipped", !anim.GetBool("WeaponEquipped")); 
    }
}
