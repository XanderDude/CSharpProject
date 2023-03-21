using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseClass : MonoBehaviour
{
    [SerializeField] protected Transform weaponAttackSpawn; //spawn for projectile
    [SerializeField] protected GameObject projectilePrefab; //the prefab projectile that's fired
    [SerializeField] private int damage = 10;
    [SerializeField] private float fireRate = 0.5f; //time to wait till before next bullet can be fired
    private bool fired = false;
    [SerializeField] private float reloadSpeed = 2f; //time it takes to refill mag ammo when reloading
    [SerializeField] private int magSize = 50;
    private int currentMagAmmo;
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

        }
    }
    [SerializeField] private int reserveSize = 200;
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
            //Debug.Log(ammoCurrentReserve);
        }
    }
    private bool reloading = false; //if currently reloading

    private ObjectPool pool;
    public bool infiniteAmmo = false;

    public event Action<float, int, int> OnPlayerReload; //Publisher: action for showing the reloading hud notif. Reload time, current mag ammo, current reserve ammo.
    public event Action<int, int> OnPlayerShoot; //Publisher: action for updating hud when shooting, mag capacity and reserve

    private void Start()
    {
        currentMagAmmo = magSize;
        currentReserveAmmo = reserveSize;
    }

    public void Reload()
    {
        if (!reloading && currentReserveAmmo > 0)
        {
            StartCoroutine(ReloadDelay());
        }

    }
    private IEnumerator ReloadDelay()
    {
        OnPlayerReload?.Invoke(reloadSpeed, 0, currentReserveAmmo);
        reloading = true;
        //pHUD.HUDToggleReloading(reloading); //turn on reloading text
        yield return new WaitForSeconds(reloadSpeed); //wait according to reload speed
        ReloadMag(); //then add ammo to mag
        reloading = false; //the reload has completed
        //pHUD.HUDToggleReloading(reloading); //turn off reloading text
    }

    private void ReloadMag()
    {
        //This ensures reserve ammo only loses what's needed to refill mag, and the mag only gets what's available in reserve
        //Also checks if mag is empty to prevent magically reloading round in chamber
        int ammoNeeded = currentMagAmmo == 0 ? //To calculate ammo needed to fill mag: if mag is empty..
            (magSize - 1) - currentMagAmmo : //..ammo needed is max minus round in chamber, else..
            magSize - currentMagAmmo; //..the ammo needed is max minus what's in mag
        CurrentMagAmmo = currentReserveAmmo; //add what's in reserve to mag. Adding reserve for when reserve is not enough to fill mag to max
        CurrentReserveAmmo = -(ammoNeeded); //subtract from reserve what's needed to fill mag.
        OnPlayerShoot?.Invoke(currentMagAmmo, currentReserveAmmo); //invoke weapon reloading event if it exists
    }

    public void Shoot(bool raycastHit, RaycastHit hit, Transform cameraTrans)
    {
        if (reloading || fired || !raycastHit)
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
        StartCoroutine(FireRate());
        if (hit.collider.TryGetComponent<EnemyHealth>(out EnemyHealth enemy)) //if raycast hits, try to grab enemy health
        {
            enemy.Damage(damage); //pass damage to enemy Damage function
            Debug.Log("Enemy Hit");
        }
        if (!infiniteAmmo)
        {
            CurrentMagAmmo = -1; //if infinite ammo is off, subract one from mag (Fires the gun)
            OnPlayerShoot?.Invoke(currentMagAmmo, currentReserveAmmo); //invoke weapon reloading event if it exists
        }
    }



    private IEnumerator FireRate() //called in update when gun is fired
    {
        fired = true; //start fired cooldown
        yield return new WaitForSeconds(fireRate);
        fired = false; //end fired cooldown
        Debug.Log("Firerate ended is " + fired);
    }
}
