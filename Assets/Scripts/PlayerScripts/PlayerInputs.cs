using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInputs : MonoBehaviour
{
    private GameManager manager;
    private Transform cameraTrans; //pos and rotation of main camera

    public enum WeaponType { MachineGun, RocketLauncher, Sword, Pistol, Shotgun}; //The types of weapons that the player can switch to
    public WeaponType playerWeapon; //a reference to the weapon types
    public WeaponBaseClass[] weapons;
    public WeaponBaseClass currentWeapon;

    public bool infiniteAmmo = false;

    public event Action<string, int, int> OnPlayerSwitchWeapons;

    // Start is called before the first frame update
    void Start()
    {
        cameraTrans = Camera.main.transform;
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        weapons = GetComponentsInChildren<WeaponBaseClass>(true);
        currentWeapon = weapons[(int)playerWeapon];
        currentWeapon.SwitchWeaponAnim();
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
            else
            {
                if (currentWeapon.triggerPressed)
                    currentWeapon.triggerPressed = false;
            }
            if (Input.GetKeyDown(KeyCode.R))//if player wants to reload
            {

                currentWeapon.Reload();
            }
            if (currentWeapon.reloading)
            {
                return; //cannot switch weapons if reloading
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && playerWeapon != WeaponType.MachineGun)
            {
                currentWeapon.SwitchWeaponAnim();
                currentWeapon = weapons[(int)WeaponType.MachineGun];
                playerWeapon = WeaponType.MachineGun;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && playerWeapon != WeaponType.RocketLauncher)
            {
                currentWeapon.SwitchWeaponAnim();
                currentWeapon = weapons[(int)WeaponType.RocketLauncher];
                playerWeapon = WeaponType.RocketLauncher;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && playerWeapon != WeaponType.Sword)
            {
                currentWeapon.SwitchWeaponAnim();
                currentWeapon = weapons[(int)WeaponType.Sword];
                playerWeapon = WeaponType.Sword;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentWeapon.SwitchWeaponAnim();
                currentWeapon = weapons[(int)WeaponType.Pistol];
                playerWeapon = WeaponType.Pistol;
                SwitchWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currentWeapon.SwitchWeaponAnim();
                currentWeapon = weapons[(int)WeaponType.Shotgun];
                playerWeapon = WeaponType.Shotgun;
                SwitchWeapon();
            }
        }
    }
    
    private void SwitchWeapon()
    {
        //foreach (WeaponBaseClass weapon in weapons)
        //{
        //    if (weapon != currentWeapon)
        //    {
        //        weapon.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        weapon.gameObject.SetActive(true);
        //    }
        //}

        OnPlayerSwitchWeapons?.Invoke(Enum.GetName(typeof(WeaponType), playerWeapon), currentWeapon.CurrentMagAmmo, currentWeapon.CurrentReserveAmmo);
        currentWeapon.SwitchWeaponAnim();
    }
}
