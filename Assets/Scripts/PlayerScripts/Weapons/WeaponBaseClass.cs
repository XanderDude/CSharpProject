using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseClass : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float fireRate = 0.5f; //time to wait till before next bullet can be fired
    private float fireRateTimer = 0f;
    private bool fired = false;
    [SerializeField] private float reloadSpeed = 2f; //time it takes to refill mag ammo when reloading
    [SerializeField] private int magSize = 50;
    private int currentMagAmmo;
    [SerializeField] private int reserveSize = 200;
    private int currentReserveAmmo;
    private bool reloading = false; //if currently reloading

    private ObjectPool pool;
    private bool infiniteAmmo = false;

    [SerializeField] private PlayerHUD pHUD;

    private void Start()
    {
        currentMagAmmo = magSize;
        currentReserveAmmo = reserveSize;
    }

    public void Reload()
    {
        StartCoroutine(ReloadDelay());
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
        int ammoNeeded = currentMagAmmo == 0 ? //To calculate ammo needed to fill mag: if mag is empty..
            (magSize - 1) - currentMagAmmo : //..ammo needed is max minus round in chamber, else..
            magSize - currentMagAmmo; //..the ammo needed is max minus what's in mag
        currentMagAmmo = currentReserveAmmo; //add what's in reserve to mag. Adding reserve for when reserve is not enough to fill mag to max
        currentReserveAmmo = -(ammoNeeded); //subtract from reserve what's needed to fill mag.
    }

    public void Shoot(bool raycastHit, RaycastHit hit)
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
            reloading = true; //reloading has started
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
            currentMagAmmo = -1; //if infinite ammo is off, subract one from mag (Fires the gun)
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
